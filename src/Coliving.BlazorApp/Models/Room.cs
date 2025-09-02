using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coliving.BlazorApp.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        // Renamed from FlatId -> HomeId. Map to existing DB column "FlatId" so old migrations still work.
        [Required]
        [Column("FlatId")]
        public int HomeId { get; set; }

        // Renamed navigation property Flat -> Home
        [ForeignKey(nameof(HomeId))]
        public virtual Home Home { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PricePerMonth { get; set; }

        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Range(0, double.MaxValue)]
        public double? SizeSqm { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }
    }
}
