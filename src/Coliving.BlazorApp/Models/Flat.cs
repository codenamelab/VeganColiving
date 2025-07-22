using System.ComponentModel.DataAnnotations;

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

		// Additional fields can be added as needed, such as amenities, owner, etc.
	}
}
