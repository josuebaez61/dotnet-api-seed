using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class RequestPasswordResetValidator : AbstractValidator<RequestPasswordResetDto>
  {
    public RequestPasswordResetValidator()
    {
      RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email is required")
          .EmailAddress().WithMessage("Email must be a valid email address")
          .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");
    }
  }
}
