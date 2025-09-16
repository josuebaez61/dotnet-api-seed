using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Features.Auth.Commands.RequestPasswordReset
{
  public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, PasswordResetResponseDto>
  {
    private readonly UserManager<User> _userManager;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly ILocalizationService _localizationService;

    public RequestPasswordResetCommandHandler(
        UserManager<User> userManager,
        IAuthService authService,
        IEmailService emailService,
        ILocalizationService localizationService)
    {
      _userManager = userManager;
      _authService = authService;
      _emailService = emailService;
      _localizationService = localizationService;
    }

    public async Task<PasswordResetResponseDto> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
      var user = await _userManager.FindByEmailAsync(request.Request.Email);
      if (user == null)
      {
        // Por seguridad, no revelamos si el email existe o no
        return new PasswordResetResponseDto
        {
          ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
      }

      if (!user.IsActive)
      {
        throw new UnauthorizedAccessException(_localizationService.GetErrorMessage("AccountDeactivated"));
      }

      // Generar código de reset
      var resetCode = await _authService.GeneratePasswordResetCodeAsync(user.Id);

      // Enviar email
      await _emailService.SendPasswordResetEmailAsync(user.Email!, user.UserName!, resetCode);

      return new PasswordResetResponseDto
      {
        ExpiresAt = DateTime.UtcNow.AddMinutes(15)
      };
    }
  }
}
