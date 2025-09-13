using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Auth.Commands.VerifyEmailChange
{
  public class VerifyEmailChangeCommandHandler : IRequestHandler<VerifyEmailChangeCommand, ApiResponse>
  {
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILocalizationService _localizationService;

    public VerifyEmailChangeCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        ILocalizationService localizationService)
    {
      _context = context;
      _emailService = emailService;
      _localizationService = localizationService;
    }

    public async Task<ApiResponse> Handle(VerifyEmailChangeCommand request, CancellationToken cancellationToken)
    {
      // Buscar el código de verificación
      var verificationCode = await _context.EmailVerificationCodes
          .FirstOrDefaultAsync(evc => evc.VerificationCode == request.Request.VerificationCode
                                     && !evc.IsUsed
                                     && !evc.IsDeleted, cancellationToken);

      if (verificationCode == null)
      {
        throw new CleanArchitecture.Application.Common.Exceptions.InvalidOperationError("Invalid or expired verification code");
      }

      // Verificar que no haya expirado
      if (verificationCode.ExpiresAt < DateTime.UtcNow)
      {
        verificationCode.IsDeleted = true;
        verificationCode.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        throw new CleanArchitecture.Application.Common.Exceptions.InvalidOperationError("Verification code has expired");
      }

      // Verificar que el usuario existe
      var user = await _context.Users
          .FirstOrDefaultAsync(u => u.Id == verificationCode.UserId, cancellationToken);

      if (user == null)
      {
        throw new CleanArchitecture.Application.Common.Exceptions.UserNotFoundError(verificationCode.UserId.ToString());
      }

      // Verificar que el nuevo email no esté ya en uso por otro usuario
      var existingUser = await _context.Users
          .FirstOrDefaultAsync(u => u.Email == verificationCode.Email && u.Id != verificationCode.UserId, cancellationToken);

      if (existingUser != null)
      {
        throw new CleanArchitecture.Application.Common.Exceptions.UserAlreadyExistsError("email", verificationCode.Email);
      }

      // Actualizar el email del usuario
      var oldEmail = user.Email;
      user.Email = verificationCode.Email;
      user.NormalizedEmail = verificationCode.Email.ToUpperInvariant();
      user.EmailConfirmed = true; // Marcar como confirmado
      user.UpdatedAt = DateTime.UtcNow;

      // Marcar el código como usado
      verificationCode.IsUsed = true;
      verificationCode.UsedAt = DateTime.UtcNow;
      verificationCode.UpdatedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync(cancellationToken);

      // Enviar email de confirmación al nuevo email
      await _emailService.SendEmailChangeConfirmationEmailAsync(
          verificationCode.Email,
          user.UserName,
          oldEmail);

      var successMessage = _localizationService.GetString("Success_EmailChanged");
      return ApiResponse.SuccessResponse(successMessage);
    }
  }
}
