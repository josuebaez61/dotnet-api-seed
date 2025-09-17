using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
  public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
  {
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
      // Configure composite primary key
      builder.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

      // Configure relationships
      builder.HasOne(e => e.User)
          .WithMany(e => e.UserTokens)
          .HasForeignKey(e => e.UserId)
          .OnDelete(DeleteBehavior.Cascade);

      // Configure table name (already handled by snake_case convention)
      builder.ToTable("user_tokens");
    }
  }
}
