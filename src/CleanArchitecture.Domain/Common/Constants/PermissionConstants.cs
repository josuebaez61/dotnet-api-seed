
namespace CleanArchitecture.Domain.Common.Constants
{
  /// <summary>
  /// Constants for all system permissions
  /// Centralizes permission names to avoid typos and facilitate maintenance
  /// </summary>
  public static class PermissionConstants
  {
    /// <summary>
    /// Permission to manage roles
    /// </summary>
    public const string ManageRoles = "manage.roles";

    /// <summary>
    /// Permission to manage users
    /// </summary>
    public const string ManageUsers = "manage.users";

    /// <summary>
    /// Permission to manage user roles
    /// </summary>
    public const string ManageUserRoles = "manage.user.roles";

    /// <summary>
    /// Permission to manage role permissions
    /// </summary>
    public const string ManageRolePermissions = "manage.role.permissions";

    /// <summary>
    /// Admin permission
    /// </summary>
    public const string Admin = "admin";

    /// <summary>
    /// Super admin permission
    /// </summary>
    public const string SuperAdmin = "superAdmin";

    /// <summary>
    /// Verifies if a permission exists in the system
    /// </summary>
    /// <param name="permission">Permission to verify</param>
    /// <returns>True if the permission exists, False otherwise</returns>
    public static bool IsValidPermission(string permission)
    {
      return GetAllPermissions().Contains(permission);
    }

    private static List<string> GetAllPermissions()
    {
      return new List<string>
      {
        ManageRoles,
        ManageUsers,
        ManageUserRoles,
        ManageRolePermissions,
        Admin,
        SuperAdmin
      };
    }
  }
}
