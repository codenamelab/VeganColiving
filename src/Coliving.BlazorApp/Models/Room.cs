using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coliving.BlazorApp.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FlatId { get; set; }

        [ForeignKey(nameof(FlatId))]
        public virtual Flat Flat { get; set; } = null!;

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
