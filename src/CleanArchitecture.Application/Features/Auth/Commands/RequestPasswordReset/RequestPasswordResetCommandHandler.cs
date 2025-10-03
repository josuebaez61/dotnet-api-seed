using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Features.Auth.Commands.RequestPasswordReset
{
  public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, PasswordResetResponseDto>
  {
    private readonly UserManager<User> _userManager;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger<RequestPasswordResetCommandHandler> _logger;

    public RequestPasswordResetCommandHandler(
        UserManager<User> userManager,
        IAuthService authService,
        IEmailService emailService,
        ILocalizationService localizationService,
        ILogger<RequestPasswordResetCommandHandler> logger)
    {
      _userManager = userManager;
      _authService = authService;
      _emailService = emailService;
      _localizationService = localizationService;
      _logger = logger;
    }

    public async Task<PasswordResetResponseDto> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
      _logger.LogInformation("Password reset requested for email: {Email}", request.Request.Email);

      var user = await _userManager.FindByEmailAsync(request.Request.Email);
      if (user == null)
      {
        _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Request.Email);
        // Por seguridad, no revelamos si el email existe o no
        return new PasswordResetResponseDto
        {
          ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
      }

      if (!user.IsActive)
      {
        _logger.LogWarning("Password reset requested for inactive user: {Email}", request.Request.Email);
        throw new UnauthorizedAccessException(_localizationService.GetErrorMessage("AccountDeactivated"));
      }

      _logger.LogInformation("Generating password reset code for user: {UserId}, Email: {Email}", user.Id, user.Email);

      // Generar código de reset
      var resetCode = await _authService.GeneratePasswordResetCodeAsync(user.Id);

      _logger.LogInformation("Password reset code generated: {ResetCode} for user: {UserId}", resetCode, user.Id);

      try
      {
        // Enviar email
        _logger.LogInformation("Attempting to send password reset email to: {Email}", user.Email);
        await _emailService.SendPasswordResetEmailAsync(user.Email!, user.UserName!, resetCode);
        _logger.LogInformation("Password reset email sent successfully to: {Email}", user.Email);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to send password reset email to: {Email}", user.Email);
        throw; // Re-throw to let the caller handle the error
      }

      return new PasswordResetResponseDto
      {
        ExpiresAt = DateTime.UtcNow.AddMinutes(15)
      };
    }
  }
}
