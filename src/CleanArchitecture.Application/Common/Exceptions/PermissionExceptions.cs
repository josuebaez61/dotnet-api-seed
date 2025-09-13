using System;

namespace CleanArchitecture.Application.Common.Exceptions
{
  public class InsufficientPermissionsError : ApplicationException
  {
    public InsufficientPermissionsError(string requiredPermission)
        : base("INSUFFICIENT_PERMISSIONS", $"Insufficient permissions. Required: {requiredPermission}", new { RequiredPermission = requiredPermission })
    {
    }
  }

  public class RoleNotFoundError : ApplicationException
  {
    public RoleNotFoundError()
        : base("ROLE_NOT_FOUND", "Role not found", new { })
    {
    }

    public RoleNotFoundError(string roleName)
        : base("ROLE_NOT_FOUND", $"Role not found: {roleName}", new { RoleName = roleName })
    {
    }
  }

  public class RoleNotFoundByIdError : ApplicationException
  {
    public RoleNotFoundByIdError(Guid roleId)
        : base("ROLE_NOT_FOUND_BY_ID", $"Role not found with ID: {roleId}", new { RoleId = roleId })
    {
    }
  }

  public class RoleAlreadyExistsError : ApplicationException
  {
    public RoleAlreadyExistsError(string roleName)
        : base("ROLE_ALREADY_EXISTS", $"Role already exists: {roleName}", new { RoleName = roleName })
    {
    }
  }

  public class PermissionNotFoundError : ApplicationException
  {
    public PermissionNotFoundError()
        : base("PERMISSION_NOT_FOUND", "Permission not found", new { })
    {
    }

    public PermissionNotFoundError(string permissionName)
        : base("PERMISSION_NOT_FOUND", $"Permission not found: {permissionName}", new { PermissionName = permissionName })
    {
    }
  }

  public class PermissionNotFoundByIdError : ApplicationException
  {
    public PermissionNotFoundByIdError(Guid permissionId)
        : base("PERMISSION_NOT_FOUND_BY_ID", $"Permission not found with ID: {permissionId}", new { PermissionId = permissionId })
    {
    }
  }

  public class PermissionAlreadyExistsError : ApplicationException
  {
    public PermissionAlreadyExistsError(string permissionName)
        : base("PERMISSION_ALREADY_EXISTS", $"Permission already exists: {permissionName}", new { PermissionName = permissionName })
    {
    }
  }

  public class UserNotInRoleError : ApplicationException
  {
    public UserNotInRoleError(string userId, string roleName)
        : base("USER_NOT_IN_ROLE", $"User {userId} is not in role {roleName}", new { UserId = userId, RoleName = roleName })
    {
    }
  }

  public class RolePermissionNotFoundError : ApplicationException
  {
    public RolePermissionNotFoundError(string roleName, string permissionName)
        : base("ROLE_PERMISSION_NOT_FOUND", $"Role {roleName} does not have permission {permissionName}", new { RoleName = roleName, PermissionName = permissionName })
    {
    }
  }
}
