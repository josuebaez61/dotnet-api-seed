using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
  public class PasswordResetCodeConfiguration : IEntityTypeConfiguration<PasswordResetCode>
  {
    public void Configure(EntityTypeBuilder<PasswordResetCode> builder)
    {
      // Primary Key
      builder.HasKey(prc => prc.Id);

      // Properties
      builder.Property(prc => prc.Code)
          .IsRequired()
          .HasMaxLength(32);

      builder.Property(prc => prc.ExpiresAt)
          .IsRequired()
          .HasColumnType("timestamp with time zone");

      builder.Property(prc => prc.IsUsed)
          .IsRequired()
          .HasDefaultValue(false);

      builder.Property(prc => prc.CreatedAt)
          .IsRequired()
          .HasColumnType("timestamp with time zone");

      // Relationships
      builder.HasOne(prc => prc.User)
          .WithMany()
          .HasForeignKey(prc => prc.UserId)
          .OnDelete(DeleteBehavior.Cascade);

      // Indexes
      builder.HasIndex(prc => prc.Code)
          .IsUnique();

      builder.HasIndex(prc => prc.UserId);

      // Table name will be set by snake_case convention
    }
  }
}