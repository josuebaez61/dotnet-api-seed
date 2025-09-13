using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Permissions.Commands.CreatePermission
{
  public class CreatePermissionCommand : IRequest<PermissionDto>
  {
    public CreatePermissionDto Permission { get; set; } = new();
  }
}
