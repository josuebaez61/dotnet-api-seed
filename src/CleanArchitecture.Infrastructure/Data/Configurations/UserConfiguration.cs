using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Configure table name (already handled by snake_case convention)
            builder.ToTable("users");
            builder.HasKey(u => u.Id);

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

            // User-UserRole relationship (one-to-many)
            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        }
    }
}
