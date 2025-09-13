using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestDto>
  {
    public ChangePasswordRequestValidator()
    {
      RuleFor(x => x.CurrentPassword)
          .NotEmpty().WithMessage("Current password is required");

      RuleFor(x => x.NewPassword)
          .NotEmpty().WithMessage("New password is required")
          .MinimumLength(8).WithMessage("New password must be at least 8 characters long")
          .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
          .WithMessage("New password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");
    }
  }
}
