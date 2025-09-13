using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class VerifyEmailChangeValidator : AbstractValidator<VerifyEmailChangeDto>
  {
    public VerifyEmailChangeValidator()
    {
      RuleFor(x => x.VerificationCode)
          .NotEmpty().WithMessage("Verification code is required")
          .Length(32).WithMessage("Verification code must be 32 characters long");
    }
  }
}
