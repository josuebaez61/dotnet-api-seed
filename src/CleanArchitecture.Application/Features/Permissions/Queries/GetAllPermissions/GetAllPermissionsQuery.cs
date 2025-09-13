using System.Collections.Generic;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Permissions.Queries.GetAllPermissions
{
  public class GetAllPermissionsQuery : IRequest<List<PermissionDto>>
  {
  }
}
