using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs
{
  public class UpdateUserRolesRequestDto
  {
    [Required]
    public List<Guid> RoleIds { get; set; } = new List<Guid>();
  }
}
