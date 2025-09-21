using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs
{
  /// <summary>
  /// DTO for assigning multiple users to a role
  /// </summary>
  public class AssignUsersToRoleRequestDto
  {
    /// <summary>
    /// List of user IDs to assign to the role
    /// </summary>
    [Required]
    public List<Guid> UserIds { get; set; } = new List<Guid>();
  }
}
