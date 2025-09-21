namespace CleanArchitecture.Application.DTOs
{
  /// <summary>
  /// Represents flag URLs and information for a country
  /// </summary>
  public class CountryFlag
  {
    /// <summary>
    /// PNG format flag URL
    /// </summary>
    public Uri Png { get; set; } = new Uri("https://flagcdn.com/w320/default.png");

    /// <summary>
    /// SVG format flag URL
    /// </summary>
    public Uri Svg { get; set; } = new Uri("https://flagcdn.com/default.svg");

    /// <summary>
    /// Alt text for the flag image
    /// </summary>
    public string Alt { get; set; } = string.Empty;
  }
}
