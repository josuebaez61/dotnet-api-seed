using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs
{
  public class VerifyEmailChangeDto
  {
    public string VerificationCode { get; set; } = string.Empty;
  }
}
