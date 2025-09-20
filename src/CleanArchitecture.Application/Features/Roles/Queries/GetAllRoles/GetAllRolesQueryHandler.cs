using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetAllRoles
{
  public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
  {
    private readonly RoleManager<Role> _roleManager;
    private readonly IPermissionService _permissionService;

    public GetAllRolesQueryHandler(RoleManager<Role> roleManager, IPermissionService permissionService)
    {
      _roleManager = roleManager;
      _permissionService = permissionService;
    }

    public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
      var roles = await _roleManager.Roles.OrderByDescending(r => r.UpdatedAt).ToListAsync();

      var roleDtos = new List<RoleDto>();
      foreach (var role in roles)
      {
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

        roleDtos.Add(new RoleDto
        {
          Id = role.Id,
          Name = role.Name!,
          Description = role.Description,
          CreatedAt = role.CreatedAt,
          UpdatedAt = role.UpdatedAt,
          Permissions = permissionDtos
        });
      }

      return roleDtos;
    }
  }
}
