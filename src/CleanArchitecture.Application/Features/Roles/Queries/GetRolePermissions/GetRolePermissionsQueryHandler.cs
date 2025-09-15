using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Configurations;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetRolePermissions
{
  public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, RolePermissionsDto>
  {
    private readonly RoleManager<Role> _roleManager;
    private readonly IPermissionService _permissionService;

    public GetRolePermissionsQueryHandler(
        RoleManager<Role> roleManager,
        IPermissionService permissionService)
    {
      _roleManager = roleManager;
      _permissionService = permissionService;
    }

    public async Task<RolePermissionsDto> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
      // Get role information
      var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
      if (role == null)
      {
        throw new RoleNotFoundByIdError(request.RoleId);
      }

      // Get all system permissions
      var allPermissions = await _permissionService.GetAllPermissionsAsync();

      // Get role's assigned permissions (including hierarchical)
      var rolePermissions = await _permissionService.GetRolePermissionsAsync(request.RoleId);
      var assignedPermissionNames = rolePermissions.Select(p => p.Name).ToHashSet();

      // Build the response
      var response = new RolePermissionsDto
      {
        RoleId = role.Id,
        RoleName = role.Name!,
        RoleDescription = role.Description,
        Permissions = new List<RolePermissionItemDto>()
      };

      // Process each system permission
      foreach (var permission in allPermissions)
      {
        var permissionItem = new RolePermissionItemDto
        {
          Id = permission.Id,
          Name = permission.Name,
          Description = permission.Description,
          Resource = permission.Resource,
          Action = permission.Action,
          Module = permission.Module,
          IsHierarchical = permission.IsHierarchical,
          IsAssigned = assignedPermissionNames.Contains(permission.Name),
          IsIncludedByHierarchy = false,
          IncludesPermissions = new List<string>(),
          ParentPermissions = new List<string>()
        };

        // Check if this permission is included by hierarchy
        var parentPermissions = HierarchicalPermissionConfiguration.GetParentPermissions(permission.Name);
        foreach (var parentPermission in parentPermissions)
        {
          if (assignedPermissionNames.Contains(parentPermission))
          {
            permissionItem.IsIncludedByHierarchy = true;
            permissionItem.IncludedBy = parentPermission;
            break; // Only show the first parent that includes it
          }
        }

        // Get permissions that this permission includes hierarchically
        permissionItem.IncludesPermissions = HierarchicalPermissionConfiguration.GetHierarchicalPermissions(permission.Name);

        // Get parent permissions that include this one
        permissionItem.ParentPermissions = parentPermissions;

        response.Permissions.Add(permissionItem);
      }

      // Calculate statistics
      response.TotalPermissions = response.Permissions.Count;
      response.HierarchicalPermissions = response.Permissions.Count(p => p.IsHierarchical);

      return response;
    }
  }
}
