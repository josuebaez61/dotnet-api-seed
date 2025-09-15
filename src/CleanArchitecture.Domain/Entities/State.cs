using System;

namespace CleanArchitecture.Domain.Entities
{
  public class State
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CountryId { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string? FipsCode { get; set; }
    public string? Iso2 { get; set; }
    public string? Iso31662 { get; set; }
    public string? Type { get; set; }
    public int? Level { get; set; }
    public Guid? ParentId { get; set; }
    public string? Native { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Timezone { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Flag { get; set; } = true;

    // Navigation properties
    public virtual Country Country { get; set; } = null!;
  }
}
