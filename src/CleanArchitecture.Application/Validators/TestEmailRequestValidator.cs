using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class TestEmailRequestValidator : AbstractValidator<TestEmailRequestDto>
  {
    public TestEmailRequestValidator()
    {
      RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email is required")
          .EmailAddress().WithMessage("Invalid email format")
          .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

      RuleFor(x => x.EmailType)
          .IsInEnum().WithMessage("Invalid email type");
    }
  }
}
