namespace Coliving.BlazorApp.Models
{
	// Lifecycle/status of a Home within the coliving app
	public enum HomeStatus
	{
		Potential = 0, // Newly proposed / not yet an established coliving home
		Existing = 1,    // Confirmed / existing coliving home
		Inactive = 2   // (Optional) no longer active but kept for history
	}
}
