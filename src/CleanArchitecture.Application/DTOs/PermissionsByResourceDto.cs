using System.Collections.Generic;

namespace CleanArchitecture.Application.DTOs
{
  /// <summary>
  /// DTO representing permissions grouped by resource
  /// </summary>
  public class PermissionsByResourceDto
  {
    /// <summary>
    /// Resource name (e.g., "users", "roles", "system")
    /// </summary>
    public string Resource { get; set; } = string.Empty;

    /// <summary>
    /// Order of the resource as defined in PermissionResource.GetOrderedResources()
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// List of permissions for this resource
    /// </summary>
    public List<PermissionDto> Permissions { get; set; } = new();
  }
}
