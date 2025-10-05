using CleanArchitecture.Application.Common.Constants;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class NewPasswordValidator : AbstractValidator<string>
  {
    public NewPasswordValidator(ILocalizationService localizationService)
    {
      RuleFor(x => x)
          .NotEmpty()
            .WithMessage(errorMessage: localizationService.GetValidationMessage("NEW_PASSWORD_REQUIRED"))
          .MinimumLength(ValidationConstants.PASSWORD_MIN_LENGTH)
            .WithMessage(errorMessage: localizationService.GetValidationMessage("NEW_PASSWORD_MIN_LENGTH", [ValidationConstants.PASSWORD_MIN_LENGTH]))
          .Matches(ValidationConstants.PASSWORD_REGEX)
            .WithMessage(errorMessage: localizationService.GetValidationMessage("NEW_PASSWORD_REGEX"));
    }
  }
}
