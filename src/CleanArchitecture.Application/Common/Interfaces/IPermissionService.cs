using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces
{
  public interface IPermissionService
  {
    /// <summary>
    /// Gets all permissions for a specific user through their roles
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Collection of permissions</returns>
    Task<ICollection<Permission>> GetUserPermissionsAsync(Guid userId);

    /// <summary>
    /// Gets all permissions for a specific role
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <returns>List of permissions</returns>
    Task<List<Permission>> GetRolePermissionsAsync(Guid roleId);

    /// <summary>
    /// Checks if a user has a specific permission
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="permissionName">The permission name to check</param>
    /// <returns>True if user has the permission</returns>
    Task<bool> UserHasPermissionAsync(Guid userId, string permissionName);

    /// <summary>
    /// Checks if a user has any of the specified permissions
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="permissionNames">The permission names to check</param>
    /// <returns>True if user has any of the permissions</returns>
    Task<bool> UserHasAnyPermissionAsync(Guid userId, params string[] permissionNames);

    /// <summary>
    /// Gets all users that have a specific permission
    /// </summary>
    /// <param name="permissionName">The permission name</param>
    /// <returns>Collection of users with the permission</returns>
    Task<ICollection<User>> GetUsersWithPermissionAsync(string permissionName);

    /// <summary>
    /// Checks if role has a specific permission (including hierarchical)
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="permissionName">The permission name to check</param>
    /// <returns>True if role has the permission</returns>
    Task<bool> RoleHasPermissionAsync(Guid roleId, string permissionName);

    /// <summary>
    /// Checks if user has a specific permission by resource and action (including hierarchical)
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="resource">The resource</param>
    /// <param name="action">The action</param>
    /// <returns>True if user has the permission</returns>
    Task<bool> HasPermissionAsync(Guid userId, string resource, string action);

    /// <summary>
    /// Checks if user has a specific permission (including hierarchical)
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="permissionName">The permission name</param>
    /// <returns>True if user has the permission</returns>
    Task<bool> HasPermissionAsync(Guid userId, string permissionName);

    /// <summary>
    /// Assigns a permission to a role
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="permissionId">The permission ID</param>
    /// <returns>True if successful</returns>
    Task<bool> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId);

    /// <summary>
    /// Removes a permission from a role
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="permissionId">The permission ID</param>
    /// <returns>True if successful</returns>
    Task<bool> RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId);

    /// <summary>
    /// Updates a permission
    /// </summary>
    /// <param name="permissionId">The permission ID</param>
    /// <param name="permission">The permission data</param>
    /// <returns>The updated permission or null if not found</returns>
    Task<Permission?> UpdatePermissionAsync(Guid permissionId, Permission permission);

    /// <summary>
    /// Deletes a permission
    /// </summary>
    /// <param name="permissionId">The permission ID</param>
    /// <returns>True if successful</returns>
    Task<bool> DeletePermissionAsync(Guid permissionId);

    /// <summary>
    /// Gets a permission by ID
    /// </summary>
    /// <param name="id">The permission ID</param>
    /// <returns>The permission or null if not found</returns>
    Task<Permission?> GetPermissionByIdAsync(Guid id);

    /// <summary>
    /// Gets a permission by name
    /// </summary>
    /// <param name="name">The permission name</param>
    /// <returns>The permission or null if not found</returns>
    Task<Permission?> GetPermissionByNameAsync(string name);

    /// <summary>
    /// Gets all permissions
    /// </summary>
    /// <returns>List of all permissions</returns>
    Task<List<Permission>> GetAllPermissionsAsync();

    /// <summary>
    /// Creates a new permission
    /// </summary>
    /// <param name="permission">The permission to create</param>
    /// <returns>The created permission</returns>
    Task<Permission> CreatePermissionAsync(Permission permission);
  }
}
