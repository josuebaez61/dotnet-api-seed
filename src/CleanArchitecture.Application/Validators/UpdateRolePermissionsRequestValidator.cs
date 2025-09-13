using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators
{
  public class UpdateRolePermissionsRequestValidator : AbstractValidator<UpdateRolePermissionsRequestDto>
  {
    public UpdateRolePermissionsRequestValidator()
    {
      RuleFor(x => x.PermissionIds)
          .NotNull().WithMessage("PermissionIds is required")
          .NotEmpty().WithMessage("PermissionIds is required");

      RuleForEach(x => x.PermissionIds)
          .NotEmpty().WithMessage("PermissionId is required");
    }
  }
}
