using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
  public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
  {
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
      // Primary Key
      builder.HasKey(p => p.Id);

      // Properties
      builder.Property(p => p.Name)
          .IsRequired()
          .HasMaxLength(100);

      builder.Property(p => p.Description)
          .IsRequired()
          .HasMaxLength(500);

      builder.Property(p => p.Resource)
          .IsRequired()
          .HasMaxLength(50);

      builder.Property(p => p.Action)
          .IsRequired()
          .HasMaxLength(50);

      builder.Property(p => p.Module)
          .IsRequired()
          .HasMaxLength(50);

      builder.Property(p => p.CreatedAt)
          .IsRequired()
          .HasColumnType("timestamp with time zone");

      builder.Property(p => p.IsHierarchical)
          .IsRequired()
          .HasDefaultValue(false);

      // Indexes
      builder.HasIndex(p => p.Name)
          .IsUnique();

      // Table name will be set by snake_case convention
    }
  }
}