using System;

namespace CleanArchitecture.Application.DTOs
{
    public class StateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        public string CountryCode { get; set; } = string.Empty;
        public string FipsCode { get; set; } = string.Empty;
        public string Iso2 { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Timezone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Flag { get; set; }
        public string Iso31662 { get; set; } = string.Empty;
        public int? Level { get; set; }
        public int? ParentId { get; set; }
        public string Native { get; set; } = string.Empty;
    }
}
