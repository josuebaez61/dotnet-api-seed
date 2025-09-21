namespace CleanArchitecture.Application.DTOs
{
  /// <summary>
  /// DTO for phone code information used in phone number selectors
  /// </summary>
  public class PhoneCodeDto
  {
    /// <summary>
    /// Country ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Country name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ISO2 country code (e.g., "US", "MX", "ES")
    /// </summary>
    public string Iso2 { get; set; } = string.Empty;

    /// <summary>
    /// Phone country code (e.g., "+1", "+52", "+34")
    /// </summary>
    public string PhoneCode { get; set; } = string.Empty;

    /// <summary>
    /// Country flag emoji
    /// </summary>
    public string Emoji { get; set; } = string.Empty;

    /// <summary>
    /// Country flag URLs and information
    /// </summary>
    public CountryFlag Flags => new CountryFlag
    {
      Png = new Uri($"https://flagcdn.com/w320/{this.Iso2.ToLower()}.png"),
      Svg = new Uri($"https://flagcdn.com/{this.Iso2.ToLower()}.svg"),
      Alt = $"{this.Name}'s flag",
    };
  }
}
