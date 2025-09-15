using System.Text.Json.Serialization;

namespace CleanArchitecture.Infrastructure.Data.Seeds.Models
{
  public class CountryJsonDto
  {
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("iso3")]
    public string? Iso3 { get; set; }

    [JsonPropertyName("iso2")]
    public string? Iso2 { get; set; }

    [JsonPropertyName("numeric_code")]
    public string? NumericCode { get; set; }

    [JsonPropertyName("phonecode")]
    public string? Phonecode { get; set; }

    [JsonPropertyName("capital")]
    public string? Capital { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("currency_name")]
    public string? CurrencyName { get; set; }

    [JsonPropertyName("currency_symbol")]
    public string? CurrencySymbol { get; set; }

    [JsonPropertyName("tld")]
    public string? Tld { get; set; }

    [JsonPropertyName("native")]
    public string? Native { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("subregion")]
    public string? Subregion { get; set; }

    [JsonPropertyName("nationality")]
    public string? Nationality { get; set; }

    [JsonPropertyName("timezones")]
    public List<TimezoneDto>? Timezones { get; set; }

    [JsonPropertyName("translations")]
    public Dictionary<string, string>? Translations { get; set; }

    [JsonPropertyName("latitude")]
    public string? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public string? Longitude { get; set; }

    // Note: We're omitting region_id, subregion_id, wikiDataId, and emoji as requested
  }

  public class TimezoneDto
  {
    [JsonPropertyName("zoneName")]
    public string? ZoneName { get; set; }

    [JsonPropertyName("gmtOffset")]
    public int? GmtOffset { get; set; }

    [JsonPropertyName("gmtOffsetName")]
    public string? GmtOffsetName { get; set; }

    [JsonPropertyName("abbreviation")]
    public string? Abbreviation { get; set; }

    [JsonPropertyName("tzName")]
    public string? TzName { get; set; }
  }
}
