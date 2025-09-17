using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
  public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
  {
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
      // Configure relationships
      builder.HasOne(e => e.Role)
          .WithMany(e => e.RoleClaims)
          .HasForeignKey(e => e.RoleId)
          .OnDelete(DeleteBehavior.Cascade);

      // Configure table name (already handled by snake_case convention)
      builder.ToTable("role_claims");
    }
  }
}
