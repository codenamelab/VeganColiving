using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Coliving.BlazorApp.Models.Matrix.Core.Models;

namespace Coliving.BlazorApp.Models
{
    /// <summary>
    /// Join entity capturing a user's engagement with a Home (Interested or Committed).
    /// Composite key: UserId + HomeId
    /// </summary>
    public class HomeEngagement
    {
        [Required]
        public int UserId { get; set; }

        // Renamed from FlatId -> HomeId. Map to existing DB column "FlatId" so old migrations still work.
        [Required]
        public int HomeId { get; set; }

        [Required]
        public EngagementStatus Status { get; set; }

        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

        // Navigation
        public ApplicationUser? User { get; set; }
        public Home? Home { get; set; }
    }

    public enum EngagementStatus
    {
        None = 0,
        Interested = 1,
        Committed = 2
    }
}
