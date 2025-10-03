using System;
using System.Collections.Generic;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetAssignableRoles
{
  public class GetAssignableRolesQuery : IRequest<List<RoleDto>>
  {
    public Guid UserId { get; set; }
  }
}
