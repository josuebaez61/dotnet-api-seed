using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Common.Services
{
  public class PermissionService : IPermissionService
  {
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;

    public PermissionService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IRepository<Permission> permissionRepository,
        IRepository<RolePermission> rolePermissionRepository)
    {
      _userManager = userManager;
      _roleManager = roleManager;
      _permissionRepository = permissionRepository;
      _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string resource, string action)
    {
      var user = await _userManager.FindByIdAsync(userId.ToString());
      if (user == null) return false;

      var userRoles = await _userManager.GetRolesAsync(user);
      if (!userRoles.Any()) return false;

      var roles = await _roleManager.Roles
          .Where(r => userRoles.Contains(r.Name!))
          .Include(r => r.RolePermissions)
          .ThenInclude(rp => rp.Permission)
          .ToListAsync();

      return roles.Any(role => role.RolePermissions.Any(rp =>
          rp.Permission.Resource == resource && rp.Permission.Action == action));
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permissionName)
    {
      var user = await _userManager.FindByIdAsync(userId.ToString());
      if (user == null) return false;

      var userRoles = await _userManager.GetRolesAsync(user);
      if (!userRoles.Any()) return false;

      var roles = await _roleManager.Roles
          .Where(r => userRoles.Contains(r.Name!))
          .Include(r => r.RolePermissions)
          .ThenInclude(rp => rp.Permission)
          .ToListAsync();

      return roles.Any(role => role.RolePermissions.Any(rp =>
          rp.Permission.Name == permissionName));
    }

    public async Task<List<Permission>> GetUserPermissionsAsync(Guid userId)
    {
      var user = await _userManager.FindByIdAsync(userId.ToString());
      if (user == null) return new List<Permission>();

      var userRoles = await _userManager.GetRolesAsync(user);
      if (!userRoles.Any()) return new List<Permission>();

      var roles = await _roleManager.Roles
          .Where(r => userRoles.Contains(r.Name!))
          .Include(r => r.RolePermissions)
          .ThenInclude(rp => rp.Permission)
          .ToListAsync();

      var permissions = new List<Permission>();
      foreach (var role in roles)
      {
        permissions.AddRange(role.RolePermissions.Select(rp => rp.Permission));
      }

      return permissions.Distinct().ToList();
    }

    public async Task<List<Permission>> GetRolePermissionsAsync(Guid roleId)
    {
      var role = await _roleManager.FindByIdAsync(roleId.ToString());
      if (role == null) return new List<Permission>();

      var rolePermissions = await _rolePermissionRepository.FindAsync(rp => rp.RoleId == roleId);
      var permissions = new List<Permission>();

      foreach (var rolePermission in rolePermissions)
      {
        var permission = await _permissionRepository.GetByIdAsync(rolePermission.PermissionId);
        if (permission != null)
        {
          permissions.Add(permission);
        }
      }

      return permissions;
    }

    public async Task<bool> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId)
    {
      var role = await _roleManager.FindByIdAsync(roleId.ToString());
      if (role == null) return false;

      var permission = await _permissionRepository.GetByIdAsync(permissionId);
      if (permission == null) return false;

      // Check if permission is already assigned
      var existingRolePermissions = await _rolePermissionRepository.FindAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
      var existingRolePermission = existingRolePermissions.FirstOrDefault();

      if (existingRolePermission != null) return true; // Already assigned

      var rolePermission = new RolePermission
      {
        Id = Guid.NewGuid(),
        RoleId = roleId,
        PermissionId = permissionId,
        CreatedAt = DateTime.UtcNow
      };

      await _rolePermissionRepository.AddAsync(rolePermission);
      return true;
    }

    public async Task<bool> RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId)
    {
      var rolePermissions = await _rolePermissionRepository.FindAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
      var rolePermission = rolePermissions.FirstOrDefault();

      if (rolePermission == null) return false;

      await _rolePermissionRepository.DeleteAsync(rolePermission);
      return true;
    }

    public async Task<List<Permission>> GetAllPermissionsAsync()
    {
      var permissions = await _permissionRepository.GetAllAsync();
      return permissions.ToList();
    }

    public async Task<Permission?> GetPermissionByIdAsync(Guid permissionId)
    {
      return await _permissionRepository.GetByIdAsync(permissionId);
    }

    public async Task<Permission?> GetPermissionByNameAsync(string name)
    {
      var permissions = await _permissionRepository.FindAsync(p => p.Name == name);
      return permissions.FirstOrDefault();
    }

    public async Task<Permission> CreatePermissionAsync(Permission permission)
    {
      permission.Id = Guid.NewGuid();
      permission.CreatedAt = DateTime.UtcNow;
      await _permissionRepository.AddAsync(permission);
      return permission;
    }

    public async Task<Permission?> UpdatePermissionAsync(Guid permissionId, Permission permission)
    {
      var existingPermission = await _permissionRepository.GetByIdAsync(permissionId);
      if (existingPermission == null) return null;

      existingPermission.Name = permission.Name;
      existingPermission.Description = permission.Description;
      existingPermission.Resource = permission.Resource;
      existingPermission.Action = permission.Action;
      existingPermission.Module = permission.Module;
      existingPermission.LastModifiedAt = DateTime.UtcNow;

      await _permissionRepository.UpdateAsync(existingPermission);
      return existingPermission;
    }

    public async Task<bool> DeletePermissionAsync(Guid permissionId)
    {
      var permission = await _permissionRepository.GetByIdAsync(permissionId);
      if (permission == null) return false;

      await _permissionRepository.DeleteAsync(permission);
      return true;
    }
  }
}
