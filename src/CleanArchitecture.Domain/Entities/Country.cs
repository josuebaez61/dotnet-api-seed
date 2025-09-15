using System;

namespace CleanArchitecture.Domain.Entities
{
  public class Country
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Iso3 { get; set; }
    public string? NumericCode { get; set; }
    public string? Iso2 { get; set; }
    public string? Phonecode { get; set; }
    public string? Capital { get; set; }
    public string? Currency { get; set; }
    public string? CurrencyName { get; set; }
    public string? CurrencySymbol { get; set; }
    public string? Tld { get; set; }
    public string? Native { get; set; }
    public string? Nationality { get; set; }
    public string? Timezones { get; set; }
    public string? Translations { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Flag { get; set; } = true;

    // Navigation properties
    public virtual ICollection<State> States { get; set; } = new List<State>();
  }
}
