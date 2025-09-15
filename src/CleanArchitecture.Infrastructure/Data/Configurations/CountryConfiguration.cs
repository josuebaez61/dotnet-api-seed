using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.Iso3)
                .HasMaxLength(50);

            builder.Property(c => c.NumericCode)
                .HasMaxLength(50);

            builder.Property(c => c.Iso2)
                .HasMaxLength(50);

            builder.Property(c => c.Phonecode)
                .HasMaxLength(255);

            builder.Property(c => c.Capital)
                .HasMaxLength(255);

            builder.Property(c => c.Currency)
                .HasMaxLength(255);

            builder.Property(c => c.CurrencyName)
                .HasMaxLength(255);

            builder.Property(c => c.CurrencySymbol)
                .HasMaxLength(255);

            builder.Property(c => c.Tld)
                .HasMaxLength(255);

            builder.Property(c => c.Native)
                .HasMaxLength(255);

            builder.Property(c => c.Nationality)
                .HasMaxLength(255);

            builder.Property(c => c.Timezones)
                .HasColumnType("text");

            builder.Property(c => c.Translations)
                .HasColumnType("text");

            builder.Property(c => c.Latitude)
                .HasColumnType("decimal(10,8)");

            builder.Property(c => c.Longitude)
                .HasColumnType("decimal(11,8)");

            builder.Property(c => c.CreatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.UpdatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.Flag)
                .IsRequired()
                .HasDefaultValue(true);

            // Indexes
            builder.HasIndex(c => c.Iso2)
                .IsUnique()
                .HasDatabaseName("ix_countries_iso2");

            builder.HasIndex(c => c.Iso3)
                .IsUnique()
                .HasDatabaseName("ix_countries_iso3");

            builder.HasIndex(c => c.NumericCode)
                .IsUnique()
                .HasDatabaseName("ix_countries_numeric_code");

            // Table name (already handled by snake_case convention)
            builder.ToTable("countries");
        }
    }
}
