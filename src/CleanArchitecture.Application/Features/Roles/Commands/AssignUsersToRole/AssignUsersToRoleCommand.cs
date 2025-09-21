using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Commands.AssignUsersToRole
{
  /// <summary>
  /// Command for assigning multiple users to a role
  /// </summary>
  public class AssignUsersToRoleCommand : IRequest<List<UserDto>>
  {
    /// <summary>
    /// ID of the role to which users will be assigned
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// List of user IDs to assign
    /// </summary>
    public List<Guid> UserIds { get; set; } = new List<Guid>();
  }
}
