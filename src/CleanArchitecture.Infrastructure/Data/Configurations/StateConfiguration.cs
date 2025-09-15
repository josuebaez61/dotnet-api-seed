using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
  public class StateConfiguration : IEntityTypeConfiguration<State>
  {
    public void Configure(EntityTypeBuilder<State> builder)
    {
      // Primary Key
      builder.HasKey(s => s.Id);

      // Configure int as primary key with auto-increment
      builder.Property(s => s.Id)
          .ValueGeneratedOnAdd();

      // Properties
      builder.Property(s => s.Name)
          .IsRequired()
          .HasMaxLength(255);

      builder.Property(s => s.CountryId)
          .IsRequired();

      builder.Property(s => s.CountryCode)
          .IsRequired()
          .HasMaxLength(50);

      builder.Property(s => s.FipsCode)
          .HasMaxLength(255);

      builder.Property(s => s.Iso2)
          .HasMaxLength(255);

      builder.Property(s => s.Iso31662)
          .HasMaxLength(255)
          .HasColumnName("iso3166_2");

      builder.Property(s => s.Type)
          .HasMaxLength(255);

      builder.Property(s => s.Level);

      builder.Property(s => s.ParentId);

      builder.Property(s => s.Native)
          .HasMaxLength(255);

      builder.Property(s => s.Latitude)
          .HasColumnType("decimal(10,8)");

      builder.Property(s => s.Longitude)
          .HasColumnType("decimal(11,8)");

      builder.Property(s => s.Timezone)
          .HasMaxLength(255);

      builder.Property(s => s.CreatedAt)
          .HasColumnType("timestamp with time zone");

      builder.Property(s => s.UpdatedAt)
          .IsRequired()
          .HasColumnType("timestamp with time zone");

      builder.Property(s => s.Flag)
          .IsRequired()
          .HasDefaultValue(true);

      // Relationships
      builder.HasOne(s => s.Country)
          .WithMany(c => c.States)
          .HasForeignKey(s => s.CountryId)
          .OnDelete(DeleteBehavior.Cascade);

      // Indexes
      builder.HasIndex(s => s.CountryId)
          .HasDatabaseName("ix_states_country_id");

      builder.HasIndex(s => s.CountryCode)
          .HasDatabaseName("ix_states_country_code");

      builder.HasIndex(s => s.Name)
          .HasDatabaseName("ix_states_name");

      // Table name (already handled by snake_case convention)
      builder.ToTable("states");
    }
  }
}
