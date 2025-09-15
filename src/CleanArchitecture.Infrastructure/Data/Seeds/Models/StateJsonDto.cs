using System.Text.Json.Serialization;

namespace CleanArchitecture.Infrastructure.Data.Seeds.Models
{
  public class StateJsonDto
  {
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("country_id")]
    public int CountryId { get; set; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }

    [JsonPropertyName("iso2")]
    public string? Iso2 { get; set; }

    [JsonPropertyName("iso3166_2")]
    public string? Iso31662 { get; set; }

    [JsonPropertyName("fips_code")]
    public string? FipsCode { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("level")]
    public string? Level { get; set; }

    [JsonPropertyName("parent_id")]
    public string? ParentId { get; set; }

    [JsonPropertyName("latitude")]
    public string? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public string? Longitude { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }
  }
}
