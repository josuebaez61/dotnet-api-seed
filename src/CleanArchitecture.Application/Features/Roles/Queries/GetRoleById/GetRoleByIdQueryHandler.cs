using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetRoleById
{
  public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
  {
    private readonly RoleManager<Role> _roleManager;
    private readonly IPermissionService _permissionService;

    public GetRoleByIdQueryHandler(RoleManager<Role> roleManager, IPermissionService permissionService)
    {
      _roleManager = roleManager;
      _permissionService = permissionService;
    }

    public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
      var role = await _roleManager.Roles
        .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

      if (role == null)
      {
        throw new RoleNotFoundByIdError(request.RoleId);
      }

      var rolePermissions = await _permissionService.GetRolePermissionsAsync(role.Id);
      var permissionDtos = rolePermissions.Select(p => new PermissionDto
      {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Resource = p.Resource,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
      }).ToList();

      return new RoleDto
      {
        Id = role.Id,
        Name = role.Name!,
        Description = role.Description,
        CreatedAt = role.CreatedAt,
        UpdatedAt = role.UpdatedAt,
        Permissions = permissionDtos
      };
    }
  }
}
