using System;
using CleanArchitecture.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Data
{
  public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      // Configure User entity
      builder.Entity<User>(entity =>
      {
        entity.ToTable("Users");
        entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        entity.Property(e => e.ProfilePicture).HasMaxLength(500);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.IsActive).IsRequired();
      });

      // Configure Role entity
      builder.Entity<Role>(entity =>
      {
        entity.ToTable("Roles");
        entity.Property(e => e.Description).HasMaxLength(500);
        entity.Property(e => e.CreatedAt).IsRequired();
      });

      // Configure UserRole entity
      builder.Entity<UserRole>(entity =>
      {
        entity.ToTable("UserRoles");
        entity.HasKey(e => new { e.UserId, e.RoleId });
        entity.HasOne(e => e.User)
                  .WithMany(e => e.UserRoles)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(e => e.Role)
                  .WithMany(e => e.UserRoles)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
      });

      // Configure UserClaim entity
      builder.Entity<UserClaim>(entity =>
      {
        entity.ToTable("UserClaims");
        entity.HasOne(e => e.User)
                  .WithMany(e => e.UserClaims)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
      });

      // Configure UserLogin entity
      builder.Entity<UserLogin>(entity =>
      {
        entity.ToTable("UserLogins");
        entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
        entity.HasOne(e => e.User)
                  .WithMany(e => e.UserLogins)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
      });

      // Configure UserToken entity
      builder.Entity<UserToken>(entity =>
      {
        entity.ToTable("UserTokens");
        entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
        entity.HasOne(e => e.User)
                  .WithMany(e => e.UserTokens)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
      });

      // Configure RoleClaim entity
      builder.Entity<RoleClaim>(entity =>
      {
        entity.ToTable("RoleClaims");
        entity.HasOne(e => e.Role)
                  .WithMany(e => e.RoleClaims)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
      });

      // Configure Permission entity
      builder.Entity<Permission>(entity =>
      {
        entity.ToTable("Permissions");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
        entity.Property(e => e.Resource).IsRequired().HasMaxLength(50);
        entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
        entity.Property(e => e.Module).IsRequired().HasMaxLength(50);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.HasIndex(e => e.Name).IsUnique();
      });

      // Configure RolePermission entity
      builder.Entity<RolePermission>(entity =>
      {
        entity.ToTable("RolePermissions");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.CreatedAt).IsRequired();

        entity.HasOne(e => e.Role)
                  .WithMany(e => e.RolePermissions)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.Permission)
                  .WithMany()
                  .HasForeignKey(e => e.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
      });

      // Configure PasswordResetCode entity
      builder.Entity<PasswordResetCode>(entity =>
      {
        entity.ToTable("PasswordResetCodes");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Code).IsRequired().HasMaxLength(6);
        entity.Property(e => e.ExpiresAt).IsRequired();
        entity.Property(e => e.IsUsed).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();

        entity.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
      });

      // Seed data logic moved to DatabaseInitializationService
    }

    // SeedData logic moved to DatabaseInitializationService for better control and flexibility
  }
}
