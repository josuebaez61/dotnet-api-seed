using System;

namespace CleanArchitecture.Application.DTOs
{
  public class RequestPasswordResetDto
  {
    public string Email { get; set; } = string.Empty;
  }

  public class ResetPasswordDto
  {
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
  }

  public class PasswordResetResponseDto
  {
    public string Message { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
  }
}
