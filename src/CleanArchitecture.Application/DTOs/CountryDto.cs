using System;

namespace CleanArchitecture.Application.DTOs
{
    public class CountryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Iso3 { get; set; } = string.Empty;
        public string NumericCode { get; set; } = string.Empty;
        public string Iso2 { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
        public string Capital { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string CurrencyName { get; set; } = string.Empty;
        public string CurrencySymbol { get; set; } = string.Empty;
        public string Tld { get; set; } = string.Empty;
        public string Native { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Emoji { get; set; } = string.Empty;
        public string EmojiU { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Flag { get; set; }
        public string WikiDataId { get; set; } = string.Empty;
    }
}
