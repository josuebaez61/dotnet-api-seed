using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs
{
  /// <summary>
  /// DTO for unassigning a user from a role
  /// </summary>
  public class UnassignUserFromRoleRequestDto
  {
    /// <summary>
    /// ID of the user to unassign from the role
    /// </summary>
    [Required]
    public Guid UserId { get; set; }
  }
}
