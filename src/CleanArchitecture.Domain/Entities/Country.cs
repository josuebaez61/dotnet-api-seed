using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain.Entities
{
    public class Country
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(3)]
        public string? Iso3 { get; set; }
        
        [MaxLength(3)]
        public string? NumericCode { get; set; }
        
        [MaxLength(2)]
        public string? Iso2 { get; set; }
        
        [MaxLength(255)]
        public string? Phonecode { get; set; }
        
        [MaxLength(255)]
        public string? Capital { get; set; }
        
        [MaxLength(255)]
        public string? Currency { get; set; }
        
        [MaxLength(255)]
        public string? CurrencyName { get; set; }
        
        [MaxLength(255)]
        public string? CurrencySymbol { get; set; }
        
        [MaxLength(255)]
        public string? Tld { get; set; }
        
        [MaxLength(255)]
        public string? Native { get; set; }
        
        [MaxLength(255)]
        public string? Nationality { get; set; }
        
        public string? Timezones { get; set; }
        
        public string? Translations { get; set; }
        
        [Column(TypeName = "decimal(10,8)")]
        public decimal? Latitude { get; set; }
        
        [Column(TypeName = "decimal(11,8)")]
        public decimal? Longitude { get; set; }
        
        public DateTime? CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public bool Flag { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<State> States { get; set; } = new List<State>();
    }
}
