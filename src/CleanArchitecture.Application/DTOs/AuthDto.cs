using System;

namespace CleanArchitecture.Application.DTOs
{
  public class LoginRequestDto
  {
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
  }

  public class RegisterRequestDto
  {
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? ProfilePicture { get; set; }
  }

  public class AuthDataDto
  {
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public AuthUserDto User { get; set; } = new();
  }

  public class ChangePasswordRequestDto
  {
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
  }

  public class FirstTimePasswordChangeRequestDto
  {
    public string NewPassword { get; set; } = string.Empty;
  }
}
