using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs
{
  public class RequestEmailChangeDto
  {
    [Required]
    [EmailAddress]
    public string NewEmail { get; set; } = string.Empty;
  }
}
