using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Permissions.Queries.GetAllPermissions
{
  public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, List<PermissionDto>>
  {
    private readonly IPermissionService _permissionService;

    public GetAllPermissionsQueryHandler(IPermissionService permissionService)
    {
      _permissionService = permissionService;
    }

    public async Task<List<PermissionDto>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
    {
      var permissions = await _permissionService.GetAllPermissionsAsync();

      return permissions.Select(p => new PermissionDto
      {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Resource = p.Resource,
        Action = p.Action,
        Module = p.Module,
        CreatedAt = p.CreatedAt,
        LastModifiedAt = p.LastModifiedAt
      }).ToList();
    }
  }
}
