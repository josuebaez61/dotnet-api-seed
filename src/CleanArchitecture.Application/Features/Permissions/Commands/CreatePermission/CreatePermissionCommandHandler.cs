using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Features.Permissions.Commands.CreatePermission
{
  public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, PermissionDto>
  {
    private readonly IPermissionService _permissionService;

    public CreatePermissionCommandHandler(IPermissionService permissionService)
    {
      _permissionService = permissionService;
    }

    public async Task<PermissionDto> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
      var permission = new Permission
      {
        Name = request.Permission.Name,
        Description = request.Permission.Description,
        Resource = request.Permission.Resource,
        Action = request.Permission.Action,
        Module = request.Permission.Module
      };

      var createdPermission = await _permissionService.CreatePermissionAsync(permission);

      return new PermissionDto
      {
        Id = createdPermission.Id,
        Name = createdPermission.Name,
        Description = createdPermission.Description,
        Resource = createdPermission.Resource,
        Action = createdPermission.Action,
        Module = createdPermission.Module,
        CreatedAt = createdPermission.CreatedAt,
        LastModifiedAt = createdPermission.LastModifiedAt
      };
    }
  }
}
