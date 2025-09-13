using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Commands.CreateRole
{
  public class CreateRoleCommand : IRequest<RoleDto>
  {
    public CreateRoleDto Role { get; set; } = new();
  }
}
