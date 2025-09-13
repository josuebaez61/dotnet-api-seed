using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Features.Auth.Commands.ResetPassword
{
  public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResponse>
  {
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly ILocalizationService _localizationService;

    public ResetPasswordCommandHandler(
        UserManager<Domain.Entities.User> userManager,
        IAuthService authService,
        IEmailService emailService,
        ILocalizationService localizationService)
    {
      _userManager = userManager;
      _authService = authService;
      _emailService = emailService;
      _localizationService = localizationService;
    }

    public async Task<ApiResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
      var user = await _userManager.FindByEmailAsync(request.Request.Email);
      if (user == null)
      {
        return ApiResponse.ErrorResponse(_localizationService.GetErrorMessage("UserNotFound"));
      }

      if (!user.IsActive)
      {
        return ApiResponse.ErrorResponse(_localizationService.GetErrorMessage("AccountDeactivated"));
      }

      // Validar código de reset
      var isValidCode = await _authService.ValidatePasswordResetCodeAsync(user.Id, request.Request.Code);
      if (!isValidCode)
      {
        return ApiResponse.ErrorResponse(_localizationService.GetErrorMessage("PasswordResetCodeInvalid"));
      }

      // Cambiar contraseña
      var token = await _userManager.GeneratePasswordResetTokenAsync(user);
      var result = await _userManager.ResetPasswordAsync(user, token, request.Request.NewPassword);

      if (!result.Succeeded)
      {
        return ApiResponse.ErrorResponse(_localizationService.GetErrorMessage("InvalidPassword"));
      }

      // Marcar código como usado
      await _authService.MarkPasswordResetCodeAsUsedAsync(user.Id, request.Request.Code);

      // Enviar email de confirmación
      await _emailService.SendPasswordChangedEmailAsync(user.Email!, user.UserName!);

      return ApiResponse.SuccessResponse(_localizationService.GetSuccessMessage("PasswordResetSuccessful"));
    }
  }
}
