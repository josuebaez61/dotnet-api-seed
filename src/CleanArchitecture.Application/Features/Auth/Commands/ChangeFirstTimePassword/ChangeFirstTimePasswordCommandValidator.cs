using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Auth.Commands.ChangeFirstTimePassword;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class ChangePasswordRequestCommandValidator : AbstractValidator<ChangeFirstTimePasswordCommand>
  {
    public ChangePasswordRequestCommandValidator(ILocalizationService localizationService)
    {
      RuleFor(x => x.Request.NewPassword)
          .SetValidator(new NewPasswordValidator(localizationService));
    }
  }
}
