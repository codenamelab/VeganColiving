using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coliving.BlazorApp.Models
{
	public class Flat
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Title { get; set; } = string.Empty;

		[StringLength(1000)]
		public string? Description { get; set; }

		[StringLength(100)]
		public string? City { get; set; }

		[StringLength(100)]
		public string? Country { get; set; }

		[Range(0, double.MaxValue)]
		public decimal PricePerMonth { get; set; }

		public DateTime DateListed { get; set; } = DateTime.UtcNow;

		[StringLength(200)]
		public string? Address { get; set; }

		[StringLength(100)]
		public string? Type { get; set; } // e.g., Apartment, House, Room

		[Range(1, 100)]
		public int Capacity { get; set; } = 1; // Number of people the home can accommodate

		[StringLength(500)]
		public string? ImageUrl { get; set; }

		// New: store image directly in DB
		public byte[]? ImageBytes { get; set; }

		[StringLength(100)]
		public string? ImageContentType { get; set; }

		// Status to distinguish potential vs existing coliving homes
		public HomeStatus Status { get; set; } = HomeStatus.Potential;

		// When the flat became Active (nullable, only set when Status == Active)
		public DateTime? DateActivatedUtc { get; set; }

		// Navigation: Rooms in this flat/home
		public virtual ICollection<Room>? Rooms { get; set; }

		// Navigation: Multiple images for this flat
		public virtual ICollection<Image>? Images { get; set; }

		// Navigation: External URLs (zero or many)
		public virtual ICollection<ExternalUrl>? ExternalUrls { get; set; }

		// Additional fields can be added as needed, such as amenities, owner, etc.
	}
}
