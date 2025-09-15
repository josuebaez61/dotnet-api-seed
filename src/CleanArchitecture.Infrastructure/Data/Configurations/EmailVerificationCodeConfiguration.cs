using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
  public class EmailVerificationCodeConfiguration : IEntityTypeConfiguration<EmailVerificationCode>
  {
    public void Configure(EntityTypeBuilder<EmailVerificationCode> builder)
    {
      // Primary Key
      builder.HasKey(evc => evc.Id);

      // Properties
      builder.Property(evc => evc.VerificationCode)
          .IsRequired()
          .HasMaxLength(32);

      builder.Property(evc => evc.Email)
          .IsRequired()
          .HasMaxLength(256);

      builder.Property(evc => evc.ExpiresAt)
          .IsRequired()
          .HasColumnType("timestamp with time zone");

      builder.Property(evc => evc.IsUsed)
          .IsRequired()
          .HasDefaultValue(false);

      builder.Property(evc => evc.CreatedAt)
          .IsRequired()
          .HasColumnType("timestamp with time zone");

      // Relationships
      builder.HasOne(evc => evc.User)
          .WithMany()
          .HasForeignKey(evc => evc.UserId)
          .OnDelete(DeleteBehavior.Cascade);

      // Indexes
      builder.HasIndex(evc => evc.VerificationCode)
          .IsUnique();

      builder.HasIndex(evc => evc.Email);

      builder.HasIndex(evc => evc.UserId);

      // Table name will be set by snake_case convention
    }
  }
}