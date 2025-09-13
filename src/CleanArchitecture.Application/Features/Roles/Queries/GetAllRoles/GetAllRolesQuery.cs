using System.Collections.Generic;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetAllRoles
{
  public class GetAllRolesQuery : IRequest<List<RoleDto>>
  {
  }
}
