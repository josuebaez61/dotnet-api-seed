using System;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Commands.UpdateRolePermissions
{
  public class UpdateRolePermissionsCommand : IRequest<Unit>
  {
    public Guid RoleId { get; set; }
    public UpdateRolePermissionsRequestDto Request { get; set; } = new UpdateRolePermissionsRequestDto();
  }
}
