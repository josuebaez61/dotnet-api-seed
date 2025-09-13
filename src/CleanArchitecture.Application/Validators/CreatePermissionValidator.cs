using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class CreatePermissionValidator : AbstractValidator<CreatePermissionDto>
  {
    public CreatePermissionValidator()
    {
      RuleFor(x => x.Name)
          .NotEmpty().WithMessage("Permission name is required")
          .MaximumLength(100).WithMessage("Permission name cannot exceed 100 characters");

      RuleFor(x => x.Description)
          .NotEmpty().WithMessage("Permission description is required")
          .MaximumLength(500).WithMessage("Permission description cannot exceed 500 characters");

      RuleFor(x => x.Resource)
          .NotEmpty().WithMessage("Resource is required")
          .MaximumLength(50).WithMessage("Resource cannot exceed 50 characters");

      RuleFor(x => x.Action)
          .NotEmpty().WithMessage("Action is required")
          .MaximumLength(50).WithMessage("Action cannot exceed 50 characters")
          .Must(action => action == "Read" || action == "Write" || action == "Delete" || action == "Execute")
          .WithMessage("Action must be one of: Read, Write, Delete, Execute");

      RuleFor(x => x.Module)
          .NotEmpty().WithMessage("Module is required")
          .MaximumLength(50).WithMessage("Module cannot exceed 50 characters");
    }
  }
}
