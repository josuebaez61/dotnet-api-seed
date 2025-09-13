using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class RequestEmailChangeValidator : AbstractValidator<RequestEmailChangeDto>
  {
    public RequestEmailChangeValidator()
    {
      RuleFor(x => x.NewEmail)
          .NotEmpty().WithMessage("Email is required")
          .EmailAddress().WithMessage("Invalid email format")
          .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");
    }
  }
}
