using System;
using System.Collections.Generic;

namespace CleanArchitecture.Application.DTOs
{
  public class RoleDto
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
  }

  public class CreateRoleDto
  {
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Guid> PermissionIds { get; set; } = new();
  }

  public class UpdateRoleDto
  {
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Guid> PermissionIds { get; set; } = new();
  }

  public class AssignPermissionsToRoleDto
  {
    public List<Guid> PermissionIds { get; set; } = new();
  }
}
