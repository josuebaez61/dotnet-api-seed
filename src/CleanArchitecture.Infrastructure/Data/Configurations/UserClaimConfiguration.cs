using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
  public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
  {
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
      // Configure relationships
      builder.HasOne(e => e.User)
          .WithMany(e => e.UserClaims)
          .HasForeignKey(e => e.UserId)
          .OnDelete(DeleteBehavior.Cascade);

      // Configure table name (already handled by snake_case convention)
      builder.ToTable("user_claims");
    }
  }
}
