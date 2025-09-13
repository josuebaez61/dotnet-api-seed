using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
  {
    public ResetPasswordValidator()
    {
      RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email is required")
          .EmailAddress().WithMessage("Email must be a valid email address")
          .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

      RuleFor(x => x.Code)
          .NotEmpty().WithMessage("Reset code is required")
          .Length(6).WithMessage("Reset code must be 6 characters long");

      RuleFor(x => x.NewPassword)
          .NotEmpty().WithMessage("New password is required")
          .MinimumLength(8).WithMessage("New password must be at least 8 characters long")
          .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
          .WithMessage("New password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");
    }
  }
}
