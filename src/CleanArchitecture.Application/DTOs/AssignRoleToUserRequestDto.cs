using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs
{
  public class AssignRoleToUserRequestDto
  {
    [Required]
    public Guid RoleId { get; set; }
  }
}
