using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces
{
  public interface IPermissionService
  {
    Task<bool> HasPermissionAsync(Guid userId, string resource, string action);
    Task<bool> HasPermissionAsync(Guid userId, string permissionName);
    Task<List<Permission>> GetUserPermissionsAsync(Guid userId);
    Task<List<Permission>> GetRolePermissionsAsync(Guid roleId);
    Task<bool> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId);
    Task<bool> RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId);
    Task<List<Permission>> GetAllPermissionsAsync();
    Task<Permission?> GetPermissionByIdAsync(Guid permissionId);
    Task<Permission?> GetPermissionByNameAsync(string name);
    Task<Permission> CreatePermissionAsync(Permission permission);
    Task<Permission?> UpdatePermissionAsync(Guid permissionId, Permission permission);
    Task<bool> DeletePermissionAsync(Guid permissionId);
  }
}
