using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs
{
  public class TestEmailRequestDto
  {
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email type is required")]
    public EmailType EmailType { get; set; }
  }

  public enum EmailType
  {
    Welcome,
    PasswordReset,
    PasswordChanged
  }
}
