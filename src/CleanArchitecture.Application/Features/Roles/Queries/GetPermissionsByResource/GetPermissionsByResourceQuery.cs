using CleanArchitecture.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetPermissionsByResource
{
    public record GetPermissionsByResourceQuery : IRequest<List<PermissionsByResourceDto>>;
}
