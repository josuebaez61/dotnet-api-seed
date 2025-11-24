using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
  public class AddressConfiguration : IEntityTypeConfiguration<Address>
  {
    public void Configure(EntityTypeBuilder<Address> builder)
    {
      // Primary Key
      builder.HasKey(a => a.Id);

      // Properties
      builder.Property(a => a.Label)
          .IsRequired()
          .HasMaxLength(100);

      builder.Property(a => a.StreetLine1)
          .IsRequired()
          .HasMaxLength(255);

      builder.Property(a => a.StreetLine2)
          .HasMaxLength(255);

      builder.Property(a => a.PostalCode)
          .HasMaxLength(20);

      builder.Property(a => a.Latitude)
          .HasColumnType("decimal(10,8)");

      builder.Property(a => a.Longitude)
          .HasColumnType("decimal(11,8)");

      builder.Property(a => a.IsPrimary)
          .IsRequired()
          .HasDefaultValue(false);

      builder.Property(a => a.CreatedAt)
          .IsRequired()
          .HasColumnType("timestamp with time zone");

      builder.Property(a => a.UpdatedAt)
          .IsRequired()
          .HasColumnType("timestamp with time zone");

      builder.Property(a => a.IsDeleted)
          .IsRequired()
          .HasDefaultValue(false);

      // Relationships
      builder.HasOne(a => a.City)
          .WithMany()
          .HasForeignKey(a => a.CityId)
          .OnDelete(DeleteBehavior.SetNull);

      builder.HasOne(a => a.State)
          .WithMany()
          .HasForeignKey(a => a.StateId)
          .OnDelete(DeleteBehavior.SetNull);

      builder.HasOne(a => a.Country)
          .WithMany()
          .HasForeignKey(a => a.CountryId)
          .OnDelete(DeleteBehavior.SetNull);

      // Indexes
      builder.HasIndex(a => a.CityId);
      builder.HasIndex(a => a.StateId);
      builder.HasIndex(a => a.CountryId);
      builder.HasIndex(a => a.PostalCode);
      builder.HasIndex(a => a.IsPrimary);
      builder.HasIndex(a => a.IsDeleted);

      // Table name will be set by snake_case convention
    }
  }
}

