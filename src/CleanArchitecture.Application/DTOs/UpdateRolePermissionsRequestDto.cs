using System;
using System.Collections.Generic;

namespace CleanArchitecture.Application.DTOs
{
  public class UpdateRolePermissionsRequestDto
  {
    public List<Guid> PermissionIds { get; set; } = new List<Guid>();
  }
}
