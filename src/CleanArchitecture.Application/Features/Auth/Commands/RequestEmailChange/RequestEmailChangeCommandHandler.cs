using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Auth.Commands.RequestEmailChange
{
  public class RequestEmailChangeCommandHandler : IRequestHandler<RequestEmailChangeCommand, Unit>
  {
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILocalizationService _localizationService;

    public RequestEmailChangeCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        ILocalizationService localizationService)
    {
      _context = context;
      _emailService = emailService;
      _localizationService = localizationService;
    }

    public async Task<Unit> Handle(RequestEmailChangeCommand request, CancellationToken cancellationToken)
    {
      // Verificar que el usuario existe
      var user = await _context.Users
          .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

      if (user == null)
      {
        throw new UserNotFoundError(request.UserId.ToString());
      }

      // Verificar que el nuevo email no esté ya en uso
      var existingUser = await _context.Users
          .FirstOrDefaultAsync(u => u.Email == request.Request.NewEmail && u.Id != request.UserId, cancellationToken);

      if (existingUser != null)
      {
        throw new UserAlreadyExistsError("email", request.Request.NewEmail);
      }

      // Verificar que el nuevo email no sea el mismo que el actual
      if (user.Email == request.Request.NewEmail)
      {
        throw new EmailCannotBeTheSameAsCurrentError(request.Request.NewEmail);
      }

      // Limpiar códigos de verificación expirados para este usuario
      var expiredCodes = await _context.EmailVerificationCodes
          .Where(evc => evc.UserId == request.UserId && evc.ExpiresAt < DateTime.UtcNow)
          .ToListAsync(cancellationToken);

      foreach (var expiredCode in expiredCodes)
      {
        expiredCode.IsDeleted = true;
        expiredCode.UpdatedAt = DateTime.UtcNow;
      }

      // Generar código de verificación
      var verificationCode = GenerateVerificationCode();
      var expiresAt = DateTime.UtcNow.AddHours(24); // 24 horas para verificar

      // Crear registro de verificación
      var emailVerificationCode = new EmailVerificationCode
      {
        Id = Guid.NewGuid(),
        UserId = request.UserId,
        Email = request.Request.NewEmail,
        VerificationCode = verificationCode,
        ExpiresAt = expiresAt,
        IsUsed = false,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        IsDeleted = false
      };

      _context.EmailVerificationCodes.Add(emailVerificationCode);
      await _context.SaveChangesAsync(cancellationToken);

      // Enviar email de verificación
      await _emailService.SendEmailChangeVerificationEmailAsync(
        request.Request.NewEmail,
        user.UserName,
        verificationCode
      );

      return Unit.Value;
    }

    private static string GenerateVerificationCode()
    {
      using var rng = RandomNumberGenerator.Create();
      var bytes = new byte[16];
      rng.GetBytes(bytes);
      return Convert.ToHexString(bytes);
    }
  }
}
