using System;
using System.Collections.Generic;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Queries.GetUserPermissions
{
  public record GetUserPermissionsQuery(Guid UserId) : IRequest<List<PermissionDto>>;
}
