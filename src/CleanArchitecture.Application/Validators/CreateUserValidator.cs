using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class CreateUserValidator : AbstractValidator<CreateUserDto>
  {
    public CreateUserValidator()
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

      // Password validation removed - passwords are now auto-generated

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
