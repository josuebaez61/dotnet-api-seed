using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Queries.GetUserRoles
{
  public record GetUserRolesQuery(Guid UserId) : IRequest<List<RoleDto>>;
}
