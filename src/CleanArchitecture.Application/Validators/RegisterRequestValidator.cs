using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
  {
    public RegisterRequestValidator()
    {
      RuleFor(x => x.FirstName)
          .NotEmpty().WithMessage("First name is required")
          .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

      RuleFor(x => x.LastName)
          .NotEmpty().WithMessage("Last name is required")
          .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

      RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email is required")
          .EmailAddress().WithMessage("Email must be a valid email address")
          .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

      RuleFor(x => x.UserName)
          .NotEmpty().WithMessage("Username is required")
          .MinimumLength(3).WithMessage("Username must be at least 3 characters long")
          .MaximumLength(50).WithMessage("Username cannot exceed 50 characters")
          .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("Username can only contain letters, numbers, underscores, and hyphens");

      RuleFor(x => x.Password)
          .NotEmpty().WithMessage("Password is required")
          .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
          .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
          .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");

      RuleFor(x => x.DateOfBirth)
          .NotEmpty().WithMessage("Date of birth is required")
          .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
          .GreaterThan(DateTime.Today.AddYears(-120)).WithMessage("Date of birth cannot be more than 120 years ago");

      RuleFor(x => x.ProfilePicture)
          .MaximumLength(500).WithMessage("Profile picture URL cannot exceed 500 characters")
          .When(x => !string.IsNullOrEmpty(x.ProfilePicture));
    }
  }
}
