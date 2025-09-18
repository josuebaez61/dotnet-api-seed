using System;

namespace CleanArchitecture.Domain.Entities
{
  public class Permission : BaseEntity
  {
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty; // e.g., "Users", "Products"
    public string Action { get; set; } = string.Empty; // e.g., "Read", "Write", "Delete"
    public string Module { get; set; } = string.Empty; // e.g., "UserManagement", "ProductManagement"
    public DateTime? LastModifiedAt { get; set; }
    public bool IsHierarchical { get; set; } = false; // Indicates if this is a hierarchical permission

    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
  }
}
