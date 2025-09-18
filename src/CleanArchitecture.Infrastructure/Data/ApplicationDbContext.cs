using System;
using System.Text.RegularExpressions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data.Configurations;
using CleanArchitecture.Infrastructure.Data.Converters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitecture.Infrastructure.Data
{
  public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>, IApplicationDbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }
    public new DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<City> Cities { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      // Apply snake_case naming convention and UTC DateTime handling to all entities
      ApplySnakeCaseNamingAndUtcDateTimeHandling(builder);

      // Apply all entity configurations automatically
      builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

      // Seed data logic moved to DatabaseInitializationService
    }

    private void ApplySnakeCaseNamingAndUtcDateTimeHandling(ModelBuilder builder)
    {
      foreach (var entity in builder.Model.GetEntityTypes())
      {
        // Set table name to snake_case
        var tableName = entity.GetTableName();
        if (!string.IsNullOrEmpty(tableName))
        {
          entity.SetTableName(ToSnakeCase(tableName));
        }

        foreach (var property in entity.GetProperties())
        {
          // Set column names to snake_case
          var columnName = property.GetColumnName();
          if (!string.IsNullOrEmpty(columnName))
          {
            property.SetColumnName(ToSnakeCase(columnName));
          }

          // Handle DateTime properties - ensure UTC conversion
          if (property.ClrType == typeof(DateTime))
          {
            property.SetValueConverter(new UtcDateTimeValueConverter());
          }

          if (property.ClrType == typeof(DateTime?))
          {
            property.SetValueConverter(new ValueConverter<DateTime?, DateTime?>(
              v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime()) : v,
              v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
            ));
          }

          // Configure UpdatedAt to be updated on save
          if (property.Name == "UpdatedAt")
          {
            property.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Save);
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
