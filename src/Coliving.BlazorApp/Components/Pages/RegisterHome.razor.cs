using System;
using System.IO;
using System.Threading.Tasks;
using Coliving.BlazorApp.Data;
using Coliving.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Coliving.BlazorApp.Components.Pages
{
    public partial class RegisterHome : ComponentBase
    {
        [Inject]
        private ColivingDbContext Db { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private Flat newFlat = new()
        {
            Capacity = 1,
            PricePerMonth = 0,
            DateListed = DateTime.UtcNow,
        };

        private string? error;
        private string? imagePreviewDataUrl;

        private async Task HandleValidSubmit()
        {
            try
            {
                newFlat.DateListed = DateTime.UtcNow;
                Db.Flats.Add(newFlat);
                await Db.SaveChangesAsync();

                // If we stored the raw image, make a local URL for later display
                if (newFlat.ImageBytes != null && !string.IsNullOrWhiteSpace(newFlat.ImageContentType))
                {
                    newFlat.ImageUrl = $"/api/flats/{newFlat.Id}/image";
                    await Db.SaveChangesAsync();
                }

                Nav.NavigateTo("/homes");
            }
            catch (Exception ex)
            {
                error = $"Failed to save flat: {ex.Message}";
            }
        }

        private async Task OnImageSelected(InputFileChangeEventArgs e)
        {
            error = null;
            imagePreviewDataUrl = null;
            var file = e.File;
            if (file is null)
            {
                newFlat.ImageBytes = null;
                newFlat.ImageContentType = null;
                return;
            }

            // Limit file size to 4MB to avoid large payloads in SignalR
            const long maxFileSize = 4 * 1024 * 1024;
            if (file.Size > maxFileSize)
            {
                error = "Image too large. Maximum size is 4 MB.";
                newFlat.ImageBytes = null;
                newFlat.ImageContentType = null;
                return;
            }

            try
            {
                using var stream = file.OpenReadStream(maxFileSize);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                newFlat.ImageBytes = ms.ToArray();
                newFlat.ImageContentType = file.ContentType;
                imagePreviewDataUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(newFlat.ImageBytes)}";
            }
            catch (Exception ex)
            {
                error = $"Failed to read image: {ex.Message}";
                newFlat.ImageBytes = null;
                newFlat.ImageContentType = null;
            }
        }

        private void ClearImage()
        {
            newFlat.ImageBytes = null;
            newFlat.ImageContentType = null;
            imagePreviewDataUrl = null;
        }
    }
}
