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

        private Flat? flat;
        private string? error;
        protected string? ImageUrl { get; private set; }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                flat = await Db.Flats
                    .Include(f => f.Rooms!)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (flat is null)
                {
                    error = "Home not found.";
                    return;
                }

                ImageUrl = !string.IsNullOrWhiteSpace(flat.ImageUrl)
                    ? flat.ImageUrl
                    : (flat.ImageBytes != null && flat.ImageBytes.Length > 0 ? $"/api/flats/{flat.Id}/image" : null);
            }
            catch (Exception ex)
            {
                error = $"Failed to load home: {ex.Message}";
            }
        }
    }
}
