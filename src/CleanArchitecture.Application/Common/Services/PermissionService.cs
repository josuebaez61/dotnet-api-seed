using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Common.Services
{
  /// <summary>
  /// Service for managing permissions and user-role relationships
  /// </summary>
  public class PermissionService : IPermissionService
  {
    private readonly IApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;

    public PermissionService(
        IApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IMapper mapper)
    {
      _context = context;
      _userManager = userManager;
      _roleManager = roleManager;
      _mapper = mapper;
    }

    /// <summary>
    /// Gets all permissions for a user through their roles
    /// </summary>
    public async Task<ICollection<Permission>> GetUserPermissionsAsync(Guid userId)
    {
      var user = await _context.Users
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role)
                  .ThenInclude(r => r.RolePermissions)
                      .ThenInclude(rp => rp.Permission)
          .FirstOrDefaultAsync(u => u.Id == userId);

      if (user == null)
        return new List<Permission>();

      var permissions = user.UserRoles
          .Where(ur => ur.Role != null)
          .SelectMany(ur => ur.Role.RolePermissions)
          .Where(rp => rp.Permission != null)
          .Select(rp => rp.Permission)
          .Distinct()
          .ToList();

      return permissions;
    }

    /// <summary>
    /// Gets all permissions for a role
    /// </summary>
    public async Task<List<Permission>> GetRolePermissionsAsync(Guid roleId)
    {
      var role = await _context.Roles
          .Include(r => r.RolePermissions)
              .ThenInclude(rp => rp.Permission)
          .FirstOrDefaultAsync(r => r.Id == roleId);

      if (role == null)
        return new List<Permission>();

      return role.RolePermissions
          .Where(rp => rp.Permission != null)
          .Select(rp => rp.Permission)
          .ToList();
    }

    /// <summary>
    /// Checks if a user has a specific permission
    /// </summary>
    public async Task<bool> UserHasPermissionAsync(Guid userId, string permissionName)
    {
      var permissions = await GetUserPermissionsAsync(userId);
      return permissions.Any(p => p.Name == permissionName);
    }

    /// <summary>
    /// Checks if a user has any of the specified permissions
    /// </summary>
    public async Task<bool> UserHasAnyPermissionAsync(Guid userId, params string[] permissionNames)
    {
      var permissions = await GetUserPermissionsAsync(userId);
      return permissions.Any(p => permissionNames.Contains(p.Name));
    }

    /// <summary>
    /// Gets all users who have a specific permission
    /// </summary>
    public async Task<ICollection<User>> GetUsersWithPermissionAsync(string permissionName)
    {
      var users = await _context.Users
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role)
                  .ThenInclude(r => r.RolePermissions)
                      .ThenInclude(rp => rp.Permission)
          .Where(u => u.UserRoles
              .Any(ur => ur.Role.RolePermissions
                  .Any(rp => rp.Permission.Name == permissionName)))
          .ToListAsync();

      return users;
    }

    /// <summary>
    /// Checks if a role has a specific permission
    /// </summary>
    public async Task<bool> RoleHasPermissionAsync(Guid roleId, string permissionName)
    {
      var permissions = await GetRolePermissionsAsync(roleId);
      return permissions.Any(p => p.Name == permissionName);
    }

    /// <summary>
    /// Checks if a user has a specific permission (overload with user object)
    /// </summary>
    public async Task<bool> HasPermissionAsync(User user, string permissionName)
    {
      return await UserHasPermissionAsync(user.Id, permissionName);
    }

    /// <summary>
    /// Checks if a user has any of the specified permissions (overload with user object)
    /// </summary>
    public async Task<bool> HasPermissionAsync(User user, params string[] permissionNames)
    {
      return await UserHasAnyPermissionAsync(user.Id, permissionNames);
    }

    /// <summary>
    /// Checks if user has a specific permission
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
      var role = await _context.Roles.FindAsync(roleId);
      var permission = await _context.Permissions.FindAsync(permissionId);

      if (role == null || permission == null)
        return false;

      var existingRolePermission = await _context.RolePermissions
          .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

      if (existingRolePermission != null)
        return true; // Already assigned

      var rolePermission = new RolePermission
      {
        Id = Guid.NewGuid(),
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
        return false;

      _context.RolePermissions.Remove(rolePermission);
      await _context.SaveChangesAsync();

      return true;
    }


    /// <summary>
    /// Gets a permission by ID
    /// </summary>
    public async Task<Permission?> GetPermissionByIdAsync(Guid permissionId)
    {
      return await _context.Permissions.FindAsync(permissionId);
    }

    /// <summary>
    /// Gets a permission by name
    /// </summary>
    public async Task<Permission?> GetPermissionByNameAsync(string permissionName)
    {
      return await _context.Permissions
          .FirstOrDefaultAsync(p => p.Name == permissionName);
    }

    /// <summary>
    /// Gets all permissions
    /// </summary>
    public async Task<List<Permission>> GetAllPermissionsAsync()
    {
      return await _context.Permissions.ToListAsync();
    }
  }
}
