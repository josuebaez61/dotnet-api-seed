using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
  {
    public LoginRequestValidator()
    {
      RuleFor(x => x.EmailOrUsername)
          .NotEmpty().WithMessage("Email or username is required")
          .MaximumLength(256).WithMessage("Email or username cannot exceed 256 characters");

      RuleFor(x => x.Password)
          .NotEmpty().WithMessage("Password is required");
    }
  }
}
