using System;

namespace CleanArchitecture.Domain.Entities
{
  public class Address : BaseEntity
  {
    public string Label { get; set; } = string.Empty; // e.g., "Home", "Work", "Billing"
    public string StreetLine1 { get; set; } = string.Empty;
    public string? StreetLine2 { get; set; }
    public string? PostalCode { get; set; }
    public int? CityId { get; set; }
    public int? StateId { get; set; }
    public int? CountryId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsPrimary { get; set; } = false;

    // Navigation properties
    public virtual City? City { get; set; }
    public virtual State? State { get; set; }
    public virtual Country? Country { get; set; }
  }
}

