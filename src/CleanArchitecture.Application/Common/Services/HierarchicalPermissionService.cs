using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Configurations;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common.Constants;
using CleanArchitecture.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Common.Services
{
  /// <summary>
  /// Service that handles hierarchical permissions logic
  /// Higher-level permissions automatically include lower-level permissions
  /// </summary>
  public class HierarchicalPermissionService : IPermissionService
  {
    private readonly IApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public HierarchicalPermissionService(
        IApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
      _context = context;
      _userManager = userManager;
      _roleManager = roleManager;
    }

    /// <summary>
    /// Gets user permissions including hierarchical permissions
    /// </summary>
    public async Task<ICollection<Permission>> GetUserPermissionsAsync(Guid userId)
    {
      var permissions = await _context.Users
          .Where(u => u.Id == userId)
          .SelectMany(u => u.UserRoles)
          .SelectMany(ur => ur.Role.RolePermissions)
          .Select(rp => rp.Permission)
          .Distinct()
          .ToListAsync();

      return ExpandHierarchicalPermissions(permissions);
    }

    /// <summary>
    /// Gets role permissions including hierarchical permissions
    /// </summary>
    public async Task<List<Permission>> GetRolePermissionsAsync(Guid roleId)
    {
      var permissions = await _context.RolePermissions
          .Where(rp => rp.RoleId == roleId)
          .Select(rp => rp.Permission)
          .ToListAsync();

      return ExpandHierarchicalPermissions(permissions);
    }

    /// <summary>
    /// Checks if user has a specific permission (including hierarchical)
    /// </summary>
    public async Task<bool> UserHasPermissionAsync(Guid userId, string permissionName)
    {
      var userPermissions = await GetUserPermissionsAsync(userId);
      return userPermissions.Any(p => p.Name == permissionName);
    }

    /// <summary>
    /// Checks if user has any of the specified permissions (including hierarchical)
    /// </summary>
    public async Task<bool> UserHasAnyPermissionAsync(Guid userId, params string[] permissionNames)
    {
      var userPermissions = await GetUserPermissionsAsync(userId);
      return userPermissions.Any(p => permissionNames.Contains(p.Name));
    }

    /// <summary>
    /// Gets all users that have a specific permission (including hierarchical)
    /// </summary>
    public async Task<ICollection<User>> GetUsersWithPermissionAsync(string permissionName)
    {
      var users = await _context.Users
          .Where(u => u.UserRoles
              .SelectMany(ur => ur.Role.RolePermissions)
              .Any(rp => rp.Permission.Name == permissionName))
          .ToListAsync();

      return users;
    }

    /// <summary>
    /// Checks if role has a specific permission (including hierarchical)
    /// </summary>
    public async Task<bool> RoleHasPermissionAsync(Guid roleId, string permissionName)
    {
      var rolePermissions = await GetRolePermissionsAsync(roleId);
      return rolePermissions.Any(p => p.Name == permissionName);
    }

    /// <summary>
    /// Checks if user has a specific permission by resource and action (including hierarchical)
    /// </summary>
    public async Task<bool> HasPermissionAsync(Guid userId, string resource, string action)
    {
      var permissionName = $"{resource}.{action}";
      return await UserHasPermissionAsync(userId, permissionName);
    }

    /// <summary>
    /// Checks if user has a specific permission (including hierarchical)
    /// </summary>
    public async Task<bool> HasPermissionAsync(Guid userId, string permissionName)
    {
      return await UserHasPermissionAsync(userId, permissionName);
    }

    /// <summary>
    /// Assigns a permission to a role
    /// </summary>
    public async Task<bool> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId)
    {
      var existingRolePermission = await _context.RolePermissions
          .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

      if (existingRolePermission != null)
        return false; // Already exists

      var rolePermission = new RolePermission
      {
        RoleId = roleId,
        PermissionId = permissionId,
        CreatedAt = DateTime.UtcNow
      };

      _context.RolePermissions.Add(rolePermission);
      await _context.SaveChangesAsync();
      return true;
    }

    /// <summary>
    /// Removes a permission from a role
    /// </summary>
    public async Task<bool> RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId)
    {
      var rolePermission = await _context.RolePermissions
          .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

      if (rolePermission == null)
        return false; // Doesn't exist

      _context.RolePermissions.Remove(rolePermission);
      await _context.SaveChangesAsync();
      return true;
    }

    /// <summary>
    /// Updates a permission
    /// </summary>
    public async Task<Permission?> UpdatePermissionAsync(Guid permissionId, Permission permission)
    {
      var existingPermission = await _context.Permissions.FindAsync(permissionId);
      if (existingPermission == null)
        return null;

      existingPermission.Name = permission.Name;
      existingPermission.Description = permission.Description;
      existingPermission.Resource = permission.Resource;
      existingPermission.Action = permission.Action;
      existingPermission.Module = permission.Module;
      existingPermission.LastModifiedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync();
      return existingPermission;
    }

    /// <summary>
    /// Deletes a permission
    /// </summary>
    public async Task<bool> DeletePermissionAsync(Guid permissionId)
    {
      var permission = await _context.Permissions.FindAsync(permissionId);
      if (permission == null)
        return false;

      _context.Permissions.Remove(permission);
      await _context.SaveChangesAsync();
      return true;
    }

    /// <summary>
    /// Expands permissions based on hierarchical rules
    /// </summary>
    private List<Permission> ExpandHierarchicalPermissions(List<Permission> basePermissions)
    {
      var expandedPermissions = new List<Permission>(basePermissions);
      var addedPermissions = new HashSet<string>();

      foreach (var permission in basePermissions)
      {
        var hierarchicalPermissions = GetHierarchicalPermissions(permission.Name);

        foreach (var hierarchicalPermission in hierarchicalPermissions)
        {
          if (!addedPermissions.Contains(hierarchicalPermission))
          {
            // Create a virtual permission object for the hierarchical permission
            var virtualPermission = CreateVirtualPermission(hierarchicalPermission);
            expandedPermissions.Add(virtualPermission);
            addedPermissions.Add(hierarchicalPermission);
          }
        }
      }

      return expandedPermissions;
    }

    /// <summary>
    /// Gets the hierarchical permissions that should be included with a given permission
    /// </summary>
    private List<string> GetHierarchicalPermissions(string permissionName)
    {
      return HierarchicalPermissionConfiguration.GetHierarchicalPermissions(permissionName);
    }

    /// <summary>
    /// Creates a virtual permission object for hierarchical permissions
    /// </summary>
    private Permission CreateVirtualPermission(string permissionName)
    {
      var parts = permissionName.Split('.');
      var resource = parts[0];
      var action = parts[1];

      return new Permission
      {
        Id = Guid.NewGuid(), // Virtual ID
        Name = permissionName,
        Description = GetPermissionDescription(permissionName),
        Resource = resource,
        Action = action,
        Module = GetModuleForResource(resource),
        CreatedAt = DateTime.UtcNow,
        IsDeleted = false,
        IsHierarchical = true // Flag to identify virtual permissions
      };
    }

    /// <summary>
    /// Gets description for a permission
    /// </summary>
    private string GetPermissionDescription(string permissionName)
    {
      return permissionName switch
      {
        PermissionConstants.Users.Read => "Read access to users (hierarchical)",
        PermissionConstants.Users.Write => "Write access to users (hierarchical)",
        PermissionConstants.Users.Update => "Update access to users (hierarchical)",
        PermissionConstants.Users.Delete => "Delete access to users (hierarchical)",
        PermissionConstants.Users.ManageRoles => "Manage user roles (hierarchical)",
        PermissionConstants.Users.ViewSensitive => "View sensitive user information (hierarchical)",

        PermissionConstants.Roles.Read => "Read access to roles (hierarchical)",
        PermissionConstants.Roles.Write => "Write access to roles (hierarchical)",
        PermissionConstants.Roles.Update => "Update access to roles (hierarchical)",
        PermissionConstants.Roles.Delete => "Delete access to roles (hierarchical)",
        PermissionConstants.Roles.ManagePermissions => "Manage role permissions (hierarchical)",

        PermissionConstants.Permissions.Read => "Read access to permissions (hierarchical)",
        PermissionConstants.Permissions.Write => "Write access to permissions (hierarchical)",
        PermissionConstants.Permissions.Update => "Update access to permissions (hierarchical)",
        PermissionConstants.Permissions.Delete => "Delete access to permissions (hierarchical)",

        PermissionConstants.System.Admin => "System administration (hierarchical)",
        PermissionConstants.System.ViewLogs => "View system logs (hierarchical)",
        PermissionConstants.System.ManageSettings => "Manage system settings (hierarchical)",
        PermissionConstants.System.Maintenance => "Execute maintenance tasks (hierarchical)",

        PermissionConstants.Audit.ViewReports => "View audit reports (hierarchical)",
        PermissionConstants.Audit.ExportData => "Export audit data (hierarchical)",

        _ => $"{permissionName} (hierarchical)"
      };
    }

    /// <summary>
    /// Gets the module for a resource
    /// </summary>
    private string GetModuleForResource(string resource)
    {
      return resource switch
      {
        "Users" => "UserManagement",
        "Roles" => "RoleManagement",
        "Permissions" => "PermissionManagement",
        "System" => "SystemAdministration",
        "Audit" => "AuditManagement",
        _ => $"{resource}Management"
      };
    }

    // Direct database access methods
    public async Task<Permission?> GetPermissionByIdAsync(Guid id) =>
        await _context.Permissions.FindAsync(id);

    public async Task<Permission?> GetPermissionByNameAsync(string name) =>
        await _context.Permissions.FirstOrDefaultAsync(p => p.Name == name);

    public async Task<List<Permission>> GetAllPermissionsAsync() =>
        await _context.Permissions.ToListAsync();

    public async Task<Permission> CreatePermissionAsync(Permission permission)
    {
      permission.CreatedAt = DateTime.UtcNow;
      _context.Permissions.Add(permission);
      await _context.SaveChangesAsync();
      return permission;
    }
  }
}
