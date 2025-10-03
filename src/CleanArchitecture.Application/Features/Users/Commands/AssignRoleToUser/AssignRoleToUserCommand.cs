using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Commands.AssignRoleToUser
{
  public record AssignRoleToUserCommand(Guid UserId, Guid RoleId) : IRequest<RoleDto>;
}
