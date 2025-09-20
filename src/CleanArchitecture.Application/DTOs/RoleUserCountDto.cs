using System;
using System.Collections.Generic;

namespace CleanArchitecture.Application.DTOs
{
  /// <summary>
  /// DTO that represents the count of users assigned to each role
  /// Structure: { [roleId]: number }
  /// </summary>
  public class RoleUserCountDto : Dictionary<Guid, int>
  {
    public RoleUserCountDto() : base() { }

    public RoleUserCountDto(IDictionary<Guid, int> dictionary) : base(dictionary) { }
  }
}
