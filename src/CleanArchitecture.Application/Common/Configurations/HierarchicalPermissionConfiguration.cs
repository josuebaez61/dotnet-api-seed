using System.Collections.Generic;
using CleanArchitecture.Domain.Common.Constants;

namespace CleanArchitecture.Application.Common.Configurations
{
  /// <summary>
  /// Configuration class that defines hierarchical permission relationships
  /// This makes it easy to modify permission hierarchies without changing code
  /// </summary>
  public static class HierarchicalPermissionConfiguration
  {
    /// <summary>
    /// Defines which permissions are included when a higher-level permission is granted
    /// Key: The higher-level permission
    /// Value: List of permissions that should be automatically included
    /// </summary>
    public static readonly Dictionary<string, List<string>> PermissionHierarchy = new()
    {
      // Users hierarchy
      [PermissionConstants.Users.Write] = new List<string>
            {
                PermissionConstants.Users.Read
            },

      [PermissionConstants.Users.Update] = new List<string>
            {
                PermissionConstants.Users.Read
            },

      [PermissionConstants.Users.Delete] = new List<string>
            {
                PermissionConstants.Users.Read
            },

      [PermissionConstants.Users.ManageRoles] = new List<string>
            {
                PermissionConstants.Users.Read,
                PermissionConstants.Users.Write,
                PermissionConstants.Users.Update,
                PermissionConstants.Roles.Read
            },

      [PermissionConstants.Users.ViewSensitive] = new List<string>
            {
                PermissionConstants.Users.Read
            },

      [PermissionConstants.Users.Manage] = new List<string>
            {
                PermissionConstants.Users.Read,
                PermissionConstants.Users.Write,
                PermissionConstants.Users.Update,
                PermissionConstants.Users.Delete,
                PermissionConstants.Users.ManageRoles,
                PermissionConstants.Users.ViewSensitive
            },

      // Roles hierarchy
      [PermissionConstants.Roles.Write] = new List<string>
            {
                PermissionConstants.Roles.Read
            },

      [PermissionConstants.Roles.Update] = new List<string>
            {
                PermissionConstants.Roles.Read
            },

      [PermissionConstants.Roles.Delete] = new List<string>
            {
                PermissionConstants.Roles.Read
            },

      [PermissionConstants.Roles.ManagePermissions] = new List<string>
            {
                PermissionConstants.Roles.Read,
                PermissionConstants.Roles.Write,
                PermissionConstants.Roles.Update,
                PermissionConstants.Permissions.Read
            },

      [PermissionConstants.Roles.Manage] = new List<string>
            {
                PermissionConstants.Roles.Read,
                PermissionConstants.Roles.Write,
                PermissionConstants.Roles.Update,
                PermissionConstants.Roles.Delete,
                PermissionConstants.Roles.ManagePermissions
            },

      // Permissions hierarchy
      [PermissionConstants.Permissions.Write] = new List<string>
            {
                PermissionConstants.Permissions.Read
            },

      [PermissionConstants.Permissions.Update] = new List<string>
            {
                PermissionConstants.Permissions.Read
            },

      [PermissionConstants.Permissions.Delete] = new List<string>
            {
                PermissionConstants.Permissions.Read
            },

      // System hierarchy
      [PermissionConstants.System.ManageSettings] = new List<string>
            {
                PermissionConstants.System.ViewLogs
            },

      [PermissionConstants.System.Maintenance] = new List<string>
            {
                PermissionConstants.System.ViewLogs,
                PermissionConstants.System.ManageSettings
            },

      [PermissionConstants.System.Admin] = new List<string>
            {
                PermissionConstants.System.ViewLogs,
                PermissionConstants.System.ManageSettings,
                PermissionConstants.System.Maintenance,
                // Admin includes all other permissions
                PermissionConstants.Users.Read,
                PermissionConstants.Users.Write,
                PermissionConstants.Users.Update,
                PermissionConstants.Users.Delete,
                PermissionConstants.Users.ManageRoles,
                PermissionConstants.Users.ViewSensitive,
                PermissionConstants.Users.Manage,
                PermissionConstants.Roles.Read,
                PermissionConstants.Roles.Write,
                PermissionConstants.Roles.Update,
                PermissionConstants.Roles.Delete,
                PermissionConstants.Roles.ManagePermissions,
                PermissionConstants.Roles.Manage,
                PermissionConstants.Permissions.Read,
                PermissionConstants.Permissions.Write,
                PermissionConstants.Permissions.Update,
                PermissionConstants.Permissions.Delete,
                PermissionConstants.Audit.ViewReports,
                PermissionConstants.Audit.ExportData
            },

      // Audit hierarchy
      [PermissionConstants.Audit.ExportData] = new List<string>
            {
                PermissionConstants.Audit.ViewReports
            }
    };

    /// <summary>
    /// Gets the hierarchical permissions for a given permission
    /// </summary>
    /// <param name="permissionName">The permission to get hierarchical permissions for</param>
    /// <returns>List of permissions that should be included</returns>
    public static List<string> GetHierarchicalPermissions(string permissionName)
    {
      return PermissionHierarchy.TryGetValue(permissionName, out var permissions)
          ? permissions
          : new List<string>();
    }

    /// <summary>
    /// Checks if a permission has hierarchical dependencies
    /// </summary>
    /// <param name="permissionName">The permission to check</param>
    /// <returns>True if the permission has hierarchical dependencies</returns>
    public static bool HasHierarchicalPermissions(string permissionName)
    {
      return PermissionHierarchy.ContainsKey(permissionName) &&
             PermissionHierarchy[permissionName].Count > 0;
    }

    /// <summary>
    /// Gets all permissions that include a specific permission as hierarchical
    /// </summary>
    /// <param name="permissionName">The permission to find parents for</param>
    /// <returns>List of permissions that include this permission hierarchically</returns>
    public static List<string> GetParentPermissions(string permissionName)
    {
      var parents = new List<string>();

      foreach (var kvp in PermissionHierarchy)
      {
        if (kvp.Value.Contains(permissionName))
        {
          parents.Add(kvp.Key);
        }
      }

      return parents;
    }

    /// <summary>
    /// Validates that all permissions in the hierarchy exist in the system
    /// </summary>
    /// <returns>List of invalid permissions found in the hierarchy</returns>
    public static List<string> ValidateHierarchy()
    {
      var allSystemPermissions = PermissionConstants.GetAllPermissions();
      var invalidPermissions = new List<string>();

      foreach (var kvp in PermissionHierarchy)
      {
        // Check if parent permission exists
        if (!allSystemPermissions.Contains(kvp.Key))
        {
          invalidPermissions.Add($"Parent permission not found: {kvp.Key}");
        }

        // Check if child permissions exist
        foreach (var childPermission in kvp.Value)
        {
          if (!allSystemPermissions.Contains(childPermission))
          {
            invalidPermissions.Add($"Child permission not found: {childPermission} (parent: {kvp.Key})");
          }
        }
      }

      return invalidPermissions;
    }
  }
}
