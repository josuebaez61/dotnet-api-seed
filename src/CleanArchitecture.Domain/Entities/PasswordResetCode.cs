using System;

namespace CleanArchitecture.Domain.Entities
{
  public class PasswordResetCode : BaseEntity
  {
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime? UsedAt { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
  }
}
