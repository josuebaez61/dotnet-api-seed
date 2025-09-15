using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
  public class CityConfiguration : IEntityTypeConfiguration<City>
  {
    public void Configure(EntityTypeBuilder<City> builder)
    {
      // Primary Key
      builder.HasKey(c => c.Id);

      // Configure int as primary key with auto-increment
      builder.Property(c => c.Id)
          .ValueGeneratedOnAdd();

      // Properties
      builder.Property(c => c.Name)
          .IsRequired()
          .HasMaxLength(255);

      builder.Property(c => c.StateId)
          .IsRequired();

      builder.Property(c => c.Code)
          .HasMaxLength(50);

      builder.Property(c => c.Type)
          .HasMaxLength(255);

      builder.Property(c => c.Latitude)
          .HasColumnType("decimal(10,8)");

      builder.Property(c => c.Longitude)
          .HasColumnType("decimal(11,8)");

      builder.Property(c => c.Timezone)
          .HasMaxLength(255);

      builder.Property(c => c.CreatedAt)
          .IsRequired()
          .HasColumnType("timestamp with time zone");

      builder.Property(c => c.UpdatedAt)
          .HasColumnType("timestamp with time zone");

      // Relationships
      builder.HasOne(c => c.State)
          .WithMany()
          .HasForeignKey(c => c.StateId)
          .OnDelete(DeleteBehavior.Cascade);

      // Indexes
      builder.HasIndex(c => c.Name);
      builder.HasIndex(c => c.StateId);
      builder.HasIndex(c => c.Code);

      // Table name will be set by snake_case convention
    }
  }
}
