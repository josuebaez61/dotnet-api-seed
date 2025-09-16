using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
  {
    public ResetPasswordValidator()
    {
      RuleFor(x => x.Code)
          .NotEmpty().WithMessage("Reset code is required")
          .MinimumLength(16).WithMessage("Reset code must be at least 16 characters long")
          .MaximumLength(32).WithMessage("Reset code cannot exceed 32 characters");

      RuleFor(x => x.NewPassword)
          .NotEmpty().WithMessage("New password is required")
          .MinimumLength(8).WithMessage("New password must be at least 8 characters long")
          .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
          .WithMessage("New password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");
    }
  }
}
