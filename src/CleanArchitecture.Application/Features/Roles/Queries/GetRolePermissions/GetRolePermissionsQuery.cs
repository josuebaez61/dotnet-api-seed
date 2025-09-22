using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetRolePermissions
{
  public class GetRolePermissionsQuery : IRequest<List<PermissionDto>>
  {
    public Guid RoleId { get; set; }
  }
}
