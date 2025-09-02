using System;
using System.Linq;
using System.Threading.Tasks;
using Coliving.BlazorApp.Data;
using Coliving.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Coliving.BlazorApp.Components.Pages
{
    public partial class HomeDetail : ComponentBase
    {
        [Parameter]
        public int id { get; set; }

        [Inject]
        private ColivingDbContext Db { get; set; } = default!;

        private Home? home;
        private string? error;
        protected string? ImageUrl { get; private set; }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                home = await Db.Homes
                    .Include(f => f.Rooms!)
                    .Include(f => f.Images)
                    .Include(f => f.ExternalUrls) // include external links
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (home is null)
                {
                    error = "Home not found.";
                    return;
                }

                ImageUrl = !string.IsNullOrWhiteSpace(home.ImageUrl)
                    ? home.ImageUrl
                    : (home.ImageBytes != null && home.ImageBytes.Length > 0 ? $"/api/homes/{home.Id}/image" : null);
            }
            catch (Exception ex)
            {
                error = $"Failed to load home: {ex.Message}";
            }
        }
    }
}
