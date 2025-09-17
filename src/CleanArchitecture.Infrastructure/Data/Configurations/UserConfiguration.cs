using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
  public class UserConfiguration : IEntityTypeConfiguration<User>
  {
    public void Configure(EntityTypeBuilder<User> builder)
    {
      // Configure properties
      builder.Property(e => e.FirstName)
          .HasMaxLength(100)
          .IsRequired();

      builder.Property(e => e.LastName)
          .HasMaxLength(100)
          .IsRequired();

      builder.Property(e => e.ProfilePicture)
          .HasMaxLength(500);

      builder.Property(e => e.CreatedAt)
          .IsRequired();

      builder.Property(e => e.UpdatedAt);

      builder.Property(e => e.IsActive)
          .IsRequired();

      builder.Property(e => e.MustChangePassword)
          .IsRequired();

      // Configure table name (already handled by snake_case convention)
      builder.ToTable("users");
    }
  }
}
