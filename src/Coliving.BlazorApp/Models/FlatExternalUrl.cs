using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coliving.BlazorApp.Models;

public class ExternalUrl
{
	[Key]
	public int Id { get; set; }

	[Required]
	public int FlatId { get; set; }

	[ForeignKey(nameof(FlatId))]
	public virtual Flat? Flat { get; set; }

	[Required]
	[StringLength(50)]
	public string Source { get; set; } = string.Empty; // e.g., Airbnb, Booking, Website name

	[Required]
	[StringLength(1000)]
	[Url]
	public string Url { get; set; } = string.Empty;

	[StringLength(200)]
	public string? Notes { get; set; }

	public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
