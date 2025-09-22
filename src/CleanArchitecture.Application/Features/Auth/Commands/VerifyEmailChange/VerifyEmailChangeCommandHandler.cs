using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Auth.Commands.VerifyEmailChange
{
  public class VerifyEmailChangeCommandHandler : IRequestHandler<VerifyEmailChangeCommand, Unit>
  {
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public VerifyEmailChangeCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService
    )
    {
      _context = context;
      _emailService = emailService;
    }

    public async Task<Unit> Handle(VerifyEmailChangeCommand request, CancellationToken cancellationToken)
    {
      // Buscar el código de verificación
      var verificationCode = await _context.EmailVerificationCodes
          .FirstOrDefaultAsync(evc => evc.VerificationCode == request.Request.VerificationCode
            && !evc.IsUsed
            && !evc.IsDeleted, cancellationToken
          );

      if (verificationCode == null)
      {
        throw new EmailVerificationCodeExpiredError();
      }

      // Verificar que no haya expirado
      if (verificationCode.ExpiresAt < DateTime.UtcNow)
      {
        verificationCode.IsDeleted = true;
        verificationCode.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        throw new EmailVerificationCodeExpiredError();
      }

      // Verificar que el usuario existe
      var user = await _context.Users
          .FirstOrDefaultAsync(u => u.Id == verificationCode.UserId, cancellationToken);

      if (user == null)
      {
        throw new UserNotFoundError(verificationCode.UserId.ToString());
      }

      // Verificar que el nuevo email no esté ya en uso por otro usuario
      var existingUser = await _context.Users
          .FirstOrDefaultAsync(u => u.Email == verificationCode.Email && u.Id != verificationCode.UserId, cancellationToken);

      if (existingUser != null)
      {
        throw new UserAlreadyExistsError("email", verificationCode.Email);
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

      return Unit.Value;
    }
  }
}
