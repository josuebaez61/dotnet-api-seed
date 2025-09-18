using System;

namespace CleanArchitecture.Domain.Entities
{
  public class EmailVerificationCode : BaseEntity
  {
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }

    // Navigation property
    public virtual User User { get; set; } = null!;
  }
}
