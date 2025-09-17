using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
  public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
  {
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
      // Configure composite primary key
      builder.HasKey(e => new { e.LoginProvider, e.ProviderKey });

      // Configure relationships
      builder.HasOne(e => e.User)
          .WithMany(e => e.UserLogins)
          .HasForeignKey(e => e.UserId)
          .OnDelete(DeleteBehavior.Cascade);

      // Configure table name (already handled by snake_case convention)
      builder.ToTable("user_logins");
    }
  }
}
