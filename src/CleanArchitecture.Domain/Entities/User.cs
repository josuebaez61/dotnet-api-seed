using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Domain.Entities
{
  public class User : IdentityUser<Guid>
  {
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? ProfilePicture { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool MustChangePassword { get; set; } = false;

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();

  }

  public class Role : IdentityRole<Guid>
  {
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
  }

  public class UserRole : IdentityUserRole<Guid>
  {
    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
  }

  public class UserClaim : IdentityUserClaim<Guid>
  {
    public virtual User User { get; set; } = null!;
  }

  public class UserLogin : IdentityUserLogin<Guid>
  {
    public virtual User User { get; set; } = null!;
  }

  public class UserToken : IdentityUserToken<Guid>
  {
    public virtual User User { get; set; } = null!;
  }

  public class RoleClaim : IdentityRoleClaim<Guid>
  {
    public virtual Role Role { get; set; } = null!;
  }
}
