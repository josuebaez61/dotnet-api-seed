using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetRoleUserCount
{
  public record GetRoleUserCountQuery : IRequest<RoleUserCountDto>;
}
