using System;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Commands.UpdateRole
{
  public record UpdateRoleCommand(Guid RoleId, UpdateRoleDto Role) : IRequest<RoleDto>;
}
