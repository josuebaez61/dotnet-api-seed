namespace CleanArchitecture.Domain.Common.Constants
{
  /// <summary>
  /// Constants for all system permissions
  /// Centralizes permission names to avoid typos and facilitate maintenance
  /// </summary>
  public static class PermissionConstants
  {
    #region Users Permissions

    /// <summary>
    /// Permissions related to user management
    /// </summary>
    public static class Users
    {
      /// <summary>
      /// Permission to read/list users
      /// </summary>
      public const string Read = "Users.Read";

      /// <summary>
      /// Permission to create new users
      /// </summary>
      public const string Write = "Users.Write";

      /// <summary>
      /// Permission to update existing users
      /// </summary>
      public const string Update = "Users.Update";

      /// <summary>
      /// Permission to delete users
      /// </summary>
      public const string Delete = "Users.Delete";

      /// <summary>
      /// Permission to manage user roles
      /// </summary>
      public const string ManageRoles = "Users.ManageRoles";

      /// <summary>
      /// Permission to view sensitive user information
      /// </summary>
      public const string ViewSensitive = "Users.ViewSensitive";
    }

    #endregion

    #region Roles Permissions

    /// <summary>
    /// Permissions related to role management
    /// </summary>
    public static class Roles
    {
      /// <summary>
      /// Permission to read/list roles
      /// </summary>
      public const string Read = "Roles.Read";

      /// <summary>
      /// Permission to create new roles
      /// </summary>
      public const string Write = "Roles.Write";

      /// <summary>
      /// Permission to update existing roles
      /// </summary>
      public const string Update = "Roles.Update";

      /// <summary>
      /// Permission to delete roles
      /// </summary>
      public const string Delete = "Roles.Delete";

      /// <summary>
      /// Permission to manage role permissions
      /// </summary>
      public const string ManagePermissions = "Roles.ManagePermissions";
    }

    #endregion

    #region Permissions Permissions

    /// <summary>
    /// Permissions related to permission management
    /// </summary>
    public static class Permissions
    {
      /// <summary>
      /// Permission to read/list permissions
      /// </summary>
      public const string Read = "Permissions.Read";

      /// <summary>
      /// Permission to create new permissions
      /// </summary>
      public const string Write = "Permissions.Write";

      /// <summary>
      /// Permission to update existing permissions
      /// </summary>
      public const string Update = "Permissions.Update";

      /// <summary>
      /// Permission to delete permissions
      /// </summary>
      public const string Delete = "Permissions.Delete";
    }

    #endregion

    #region System Permissions

    /// <summary>
    /// Permissions related to system administration
    /// </summary>
    public static class System
    {
      /// <summary>
      /// Permission to access the administration panel
      /// </summary>
      public const string Admin = "System.Admin";

      /// <summary>
      /// Permission to view system logs
      /// </summary>
      public const string ViewLogs = "System.ViewLogs";

      /// <summary>
      /// Permission to manage system settings
      /// </summary>
      public const string ManageSettings = "System.ManageSettings";

      /// <summary>
      /// Permission to execute maintenance tasks
      /// </summary>
      public const string Maintenance = "System.Maintenance";
    }

    #endregion

    #region Audit Permissions

    /// <summary>
    /// Permissions related to audit and reports
    /// </summary>
    public static class Audit
    {
      /// <summary>
      /// Permission to view audit reports
      /// </summary>
      public const string ViewReports = "Audit.ViewReports";

      /// <summary>
      /// Permission to export audit data
      /// </summary>
      public const string ExportData = "Audit.ExportData";
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets all system permissions as a list
    /// </summary>
    /// <returns>List of all available permissions</returns>
    public static List<string> GetAllPermissions()
    {
      return new List<string>
            {
                // Users permissions
                Users.Read,
                Users.Write,
                Users.Update,
                Users.Delete,
                Users.ManageRoles,
                Users.ViewSensitive,
                
                // Roles permissions
                Roles.Read,
                Roles.Write,
                Roles.Update,
                Roles.Delete,
                Roles.ManagePermissions,
                
                // Permissions permissions
                Permissions.Read,
                Permissions.Write,
                Permissions.Update,
                Permissions.Delete,
                
                // System permissions
                System.Admin,
                System.ViewLogs,
                System.ManageSettings,
                System.Maintenance,
                
                // Audit permissions
                Audit.ViewReports,
                Audit.ExportData
            };
    }

    /// <summary>
    /// Gets the basic permissions that a standard user should have
    /// </summary>
    /// <returns>List of basic permissions</returns>
    public static List<string> GetBasicUserPermissions()
    {
      return new List<string>
            {
                Users.Read, // Can view their own profile and list basic users
            };
    }

    /// <summary>
    /// Gets the permissions that an administrator should have
    /// </summary>
    /// <returns>List of administrator permissions</returns>
    public static List<string> GetAdminPermissions()
    {
      return GetAllPermissions();
    }

    /// <summary>
    /// Gets the permissions that a moderator should have
    /// </summary>
    /// <returns>List of moderator permissions</returns>
    public static List<string> GetModeratorPermissions()
    {
      return new List<string>
            {
                Users.Read,
                Users.Write,
                Users.Update,
                Roles.Read,
                Permissions.Read,
                Audit.ViewReports
            };
    }

    /// <summary>
    /// Verifies if a permission exists in the system
    /// </summary>
    /// <param name="permission">Permission to verify</param>
    /// <returns>True if the permission exists, False otherwise</returns>
    public static bool IsValidPermission(string permission)
    {
      return GetAllPermissions().Contains(permission);
    }

    #endregion
  }
}
