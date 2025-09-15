using System;
using System.Text.RegularExpressions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Data
{
  public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>, IApplicationDbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }


    public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<State> States { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      // Apply snake_case naming convention to all entities
      ApplySnakeCaseNaming(builder);

      // Apply entity configurations
      builder.ApplyConfiguration(new CountryConfiguration());
      builder.ApplyConfiguration(new StateConfiguration());
      builder.ApplyConfiguration(new PermissionConfiguration());
      builder.ApplyConfiguration(new RolePermissionConfiguration());
      builder.ApplyConfiguration(new PasswordResetCodeConfiguration());
      builder.ApplyConfiguration(new EmailVerificationCodeConfiguration());

      // Configure User entity properties
      builder.Entity<User>(entity =>
      {
        entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        entity.Property(e => e.ProfilePicture).HasMaxLength(500);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt);
        entity.Property(e => e.IsActive).IsRequired();
        entity.Property(e => e.MustChangePassword).IsRequired();
      });

      // Configure Role entity properties
      builder.Entity<Role>(entity =>
      {
        entity.Property(e => e.Description).HasMaxLength(500);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt);
      });

      // Configure UserRole entity
      builder.Entity<UserRole>(entity =>
      {
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
        entity.HasOne(e => e.User)
                  .WithMany(e => e.UserClaims)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
      });

      // Configure UserLogin entity
      builder.Entity<UserLogin>(entity =>
      {
        entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
        entity.HasOne(e => e.User)
                  .WithMany(e => e.UserLogins)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
      });

      // Configure UserToken entity
      builder.Entity<UserToken>(entity =>
      {
        entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
        entity.HasOne(e => e.User)
                  .WithMany(e => e.UserTokens)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
      });

      // Configure RoleClaim entity
      builder.Entity<RoleClaim>(entity =>
      {
        entity.HasOne(e => e.Role)
                  .WithMany(e => e.RoleClaims)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
      });






      // Seed data logic moved to DatabaseInitializationService
    }

    private void ApplySnakeCaseNaming(ModelBuilder builder)
    {
      foreach (var entity in builder.Model.GetEntityTypes())
      {
        // Set table name to snake_case
        var tableName = entity.GetTableName();
        if (!string.IsNullOrEmpty(tableName))
        {
          entity.SetTableName(ToSnakeCase(tableName));
        }

        // Set column names to snake_case
        foreach (var property in entity.GetProperties())
        {
          var columnName = property.GetColumnName();
          if (!string.IsNullOrEmpty(columnName))
          {
            property.SetColumnName(ToSnakeCase(columnName));
          }
        }

        // Set index names to snake_case
        foreach (var index in entity.GetIndexes())
        {
          var indexName = index.GetDatabaseName();
          if (!string.IsNullOrEmpty(indexName))
          {
            index.SetDatabaseName(ToSnakeCase(indexName));
          }
        }

        // Set foreign key constraint names to snake_case
        foreach (var foreignKey in entity.GetForeignKeys())
        {
          var constraintName = foreignKey.GetConstraintName();
          if (!string.IsNullOrEmpty(constraintName))
          {
            foreignKey.SetConstraintName(ToSnakeCase(constraintName));
          }
        }
      }
    }

    private static string ToSnakeCase(string input)
    {
      if (string.IsNullOrEmpty(input))
        return input;

      // Remove AspNet prefix from Identity tables
      if (input.StartsWith("AspNet"))
      {
        input = input.Substring(6); // Remove "AspNet" prefix
      }

      // Handle special cases for common patterns
      input = input.Replace("Id", "ID"); // Keep ID as uppercase
      input = input.Replace("Url", "URL"); // Keep URL as uppercase
      input = input.Replace("Api", "API"); // Keep API as uppercase

      // Convert PascalCase to snake_case
      return Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLowerInvariant();
    }

    // SeedData logic moved to DatabaseInitializationService for better control and flexibility
  }
}
