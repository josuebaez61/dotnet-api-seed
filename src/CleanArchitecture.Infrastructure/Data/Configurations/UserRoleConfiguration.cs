using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
  public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
  {
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
      // Configure composite primary key
      builder.HasKey(e => new { e.UserId, e.RoleId });

      // Configure relationships
      builder.HasOne(e => e.User)
          .WithMany(e => e.UserRoles)
          .HasForeignKey(e => e.UserId)
          .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(e => e.Role)
          .WithMany(e => e.UserRoles)
          .HasForeignKey(e => e.RoleId)
          .OnDelete(DeleteBehavior.Cascade);

      // Configure table name (already handled by snake_case convention)
      builder.ToTable("user_roles");
    }
  }
}
