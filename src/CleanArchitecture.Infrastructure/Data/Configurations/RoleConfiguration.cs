using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
  public class RoleConfiguration : IEntityTypeConfiguration<Role>
  {
    public void Configure(EntityTypeBuilder<Role> builder)
    {
      // Configure table name (already handled by snake_case convention)
      builder.ToTable("roles");
      builder.HasKey(r => r.Id);

      // Configure properties
      builder.Property(e => e.Description)
          .HasMaxLength(500);

      builder.Property(e => e.CreatedAt)
          .IsRequired();

      builder.Property(e => e.UpdatedAt);

      // Role-UserRole relationship (one-to-many)
      builder.HasMany(r => r.UserRoles)
          .WithOne(ur => ur.Role)
          .HasForeignKey(ur => ur.RoleId)
          .IsRequired();

      // Role-RolePermission relationship is configured in RolePermissionConfiguration
    }
  }
}
