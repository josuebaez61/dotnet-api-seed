using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs
{
  public class VerifyEmailChangeDto
  {
    [Required]
    public string VerificationCode { get; set; } = string.Empty;
  }
}
