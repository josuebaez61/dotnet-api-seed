using System;
using System.Collections.Generic;

namespace CleanArchitecture.Application.DTOs
{
  /// <summary>
  /// DTO for role permissions response with hierarchical information
  /// </summary>
  public class RolePermissionsDto
  {
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? RoleDescription { get; set; }
    public List<RolePermissionItemDto> Permissions { get; set; } = new List<RolePermissionItemDto>();
    public int TotalPermissions { get; set; }
    public int HierarchicalPermissions { get; set; }
  }

  /// <summary>
  /// DTO for individual permission item with hierarchical information
  /// </summary>
  public class RolePermissionItemDto
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public bool IsHierarchical { get; set; }
    public bool IsAssigned { get; set; }
    public bool IsIncludedByHierarchy { get; set; }
    public string? IncludedBy { get; set; } // Permission that includes this one hierarchically
    public List<string> IncludesPermissions { get; set; } = new List<string>(); // Permissions included by this one
    public List<string> ParentPermissions { get; set; } = new List<string>(); // Permissions that include this one
  }
}
