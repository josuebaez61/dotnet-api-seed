using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain.Entities
{
  public class State
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public int CountryId { get; set; }

    [Required]
    [MaxLength(2)]
    public string CountryCode { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? FipsCode { get; set; }

    [MaxLength(255)]
    public string? Iso2 { get; set; }

    [MaxLength(10)]
    public string? Iso31662 { get; set; }

    [MaxLength(191)]
    public string? Type { get; set; }

    public int? Level { get; set; }

    public int? ParentId { get; set; }

    [MaxLength(255)]
    public string? Native { get; set; }

    [Column(TypeName = "decimal(10,8)")]
    public decimal? Latitude { get; set; }

    [Column(TypeName = "decimal(11,8)")]
    public decimal? Longitude { get; set; }

    [MaxLength(255)]
    public string? Timezone { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool Flag { get; set; } = true;

    // Navigation properties
    [ForeignKey(nameof(CountryId))]
    public virtual Country Country { get; set; } = null!;
  }
}
