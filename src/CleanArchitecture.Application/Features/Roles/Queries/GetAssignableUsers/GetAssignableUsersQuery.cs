using System;
using System.Collections.Generic;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetAssignableUsers
{
  public class GetAssignableUsersQuery : IRequest<List<UserOptionDto>>
  {
    public Guid RoleId { get; set; }
  }
}
