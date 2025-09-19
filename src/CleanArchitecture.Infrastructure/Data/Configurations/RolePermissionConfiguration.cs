using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
  public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
  {
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
      // Primary Key
      builder.HasKey(rp => rp.Id);

      // Properties
      builder.Property(rp => rp.RoleId)
          .IsRequired();

      builder.Property(rp => rp.PermissionId)
          .IsRequired();

      builder.Property(rp => rp.CreatedAt)
          .IsRequired()
          .HasColumnType("timestamp with time zone");

      // Relationships - configure both sides to avoid conflicts
      builder.HasOne(rp => rp.Role)
          .WithMany(r => r.RolePermissions)
          .HasForeignKey(rp => rp.RoleId)
          .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(rp => rp.Permission)
          .WithMany(p => p.RolePermissions)
          .HasForeignKey(rp => rp.PermissionId)
          .OnDelete(DeleteBehavior.Cascade);

      // Indexes
      builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
          .IsUnique();

      // Table name will be set by snake_case convention
    }
  }
}