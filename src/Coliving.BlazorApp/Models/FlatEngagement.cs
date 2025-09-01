using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Coliving.BlazorApp.Models.Matrix.Core.Models;

namespace Coliving.BlazorApp.Models
{
    /// <summary>
    /// Join entity capturing a user's engagement with a Flat (Interested or Committed).
    /// Composite key: UserId + FlatId
    /// </summary>
    public class FlatEngagement
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int FlatId { get; set; }

        [Required]
        public EngagementStatus Status { get; set; }

        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

        // Navigation
        public ApplicationUser? User { get; set; }
        public Flat? Flat { get; set; }
    }

    public enum EngagementStatus
    {
        None = 0,
        Interested = 1,
        Committed = 2
    }
}
