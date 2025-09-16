using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
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
      // Validar código de reset y obtener el userId
      var userId = await _authService.ValidatePasswordResetCodeAndGetUserIdAsync(request.Request.Code);

      var user = await _userManager.FindByIdAsync(userId.ToString());
      if (user == null)
      {
        throw new UserNotFoundError(userId.ToString());
      }

      if (!user.IsActive)
      {
        throw new AccountDeactivatedError(user.Id.ToString());
      }

      // Cambiar contraseña
      var token = await _userManager.GeneratePasswordResetTokenAsync(user);
      var result = await _userManager.ResetPasswordAsync(user, token, request.Request.NewPassword);

      if (!result.Succeeded)
      {
        throw new InvalidPasswordError();
      }

      // Marcar código como usado
      await _authService.MarkPasswordResetCodeAsUsedAsync(userId, request.Request.Code);

      // Enviar email de confirmación
      await _emailService.SendPasswordChangedEmailAsync(user.Email!, user.UserName!);

      return ApiResponse.SuccessResponse(_localizationService.GetSuccessMessage("PasswordResetSuccessful"));
    }
  }
}
