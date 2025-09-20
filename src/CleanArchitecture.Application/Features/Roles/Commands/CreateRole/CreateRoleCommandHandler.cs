using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Features.Roles.Commands.CreateRole
{
  public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
  {
    private readonly RoleManager<Role> _roleManager;
    private readonly IPermissionService _permissionService;

    public CreateRoleCommandHandler(RoleManager<Role> roleManager, IPermissionService permissionService)
    {
      _roleManager = roleManager;
      _permissionService = permissionService;
    }

    public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
      var role = new Role
      {
        Id = Guid.NewGuid(),
        Name = request.Role.Name,
        NormalizedName = request.Role.Name.ToUpper(),
        Description = request.Role.Description,
        CreatedAt = DateTime.UtcNow
      };

      var result = await _roleManager.CreateAsync(role);
      if (!result.Succeeded)
      {
        throw new InvalidOperationException($"Failed to create role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
      }

      // Assign permissions to role
      foreach (var permissionId in request.Role.PermissionIds)
      {
        await _permissionService.AssignPermissionToRoleAsync(role.Id, permissionId);
      }

      // Get role with permissions
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
