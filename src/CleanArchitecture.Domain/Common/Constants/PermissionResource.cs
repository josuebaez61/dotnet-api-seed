
using System.Collections.Generic;
using System.Linq;

namespace CleanArchitecture.Domain.Common.Constants
{
  /// <summary>
  /// Constants for all system permission modules
  /// </summary>
  public static class PermissionResource
  {
    public const string Users = "users";
    public const string System = "system";
    public const string Roles = "roles";
    public const string Permissions = "permissions";

    public static List<string> GetOrderedResources()
    {
      return new List<string> { System, Users, Roles, Permissions };
    }
  }
}
