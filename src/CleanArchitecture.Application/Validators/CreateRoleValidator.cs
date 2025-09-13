using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class CreateRoleValidator : AbstractValidator<CreateRoleDto>
  {
    public CreateRoleValidator()
    {
      RuleFor(x => x.Name)
          .NotEmpty().WithMessage("Role name is required")
          .MaximumLength(50).WithMessage("Role name cannot exceed 50 characters")
          .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("Role name can only contain letters, numbers, underscores, and hyphens");

      RuleFor(x => x.Description)
          .MaximumLength(500).WithMessage("Role description cannot exceed 500 characters")
          .When(x => !string.IsNullOrEmpty(x.Description));

      RuleFor(x => x.PermissionIds)
          .NotNull().WithMessage("Permission IDs list cannot be null");
    }
  }
}
