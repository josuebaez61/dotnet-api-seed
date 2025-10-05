using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class FirstTimePasswordChangeRequestValidator : AbstractValidator<FirstTimePasswordChangeRequestDto>
  {
    public FirstTimePasswordChangeRequestValidator(ILocalizationService localizationService)
    {
      RuleFor(x => x.NewPassword)
          .SetValidator(new NewPasswordValidator(localizationService));
    }
  }
}