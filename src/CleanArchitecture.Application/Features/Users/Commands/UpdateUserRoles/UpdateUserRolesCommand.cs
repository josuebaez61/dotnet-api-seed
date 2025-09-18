using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Commands.UpdateUserRoles
{
  public record UpdateUserRolesCommand(Guid UserId, List<Guid> RoleIds) : IRequest<List<RoleDto>>;
}
