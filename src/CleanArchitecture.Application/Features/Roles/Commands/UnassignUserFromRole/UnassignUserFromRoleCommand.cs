using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Commands.UnassignUserFromRole
{
  /// <summary>
  /// Command for unassigning a user from a role
  /// </summary>
  public class UnassignUserFromRoleCommand : IRequest<UserDto>
  {
    /// <summary>
    /// ID of the role from which the user will be unassigned
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// ID of the user to unassign
    /// </summary>
    public Guid UserId { get; set; }
  }
}
