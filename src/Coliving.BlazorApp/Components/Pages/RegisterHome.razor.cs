using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coliving.BlazorApp.Data;
using Coliving.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;

namespace Coliving.BlazorApp.Components.Pages
{
    public partial class RegisterHome : ComponentBase
    {
        [Inject]
        private ColivingDbContext Db { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private Home newHome = new()
        {
            Capacity = 1,
            PricePerMonth = 0,
            DateListed = DateTime.UtcNow,
        };

        private string? error;
        private string? imagePreviewDataUrl;
        private readonly List<(byte[] bytes, string contentType, string? title)> additionalImages = new();
        private readonly List<string> additionalImagePreviews = new();
        private readonly List<ExternalUrl> externalUrls = new() { new ExternalUrl() };

        private async Task HandleValidSubmit()
        {
            try
            {
                newHome.DateListed = DateTime.UtcNow;

                // Attach additional images to home entity
                if (additionalImages.Count > 0)
                {
                    newHome.Images ??= new List<Image>();
                    foreach (var (bytes, contentType, title) in additionalImages.Take(10))
                    {
                        newHome.Images.Add(new Image
                        {
                            Data = bytes,
                            ContentType = contentType,
                            Title = title,
                            FileName = null
                        });
                    }
                }

                // Attach external URLs (filter out empty ones)
                var cleaned = externalUrls
                    .Where(u => !string.IsNullOrWhiteSpace(u.Source) && !string.IsNullOrWhiteSpace(u.Url))
                    .ToList();
                if (cleaned.Count > 0)
                {
                    newHome.ExternalUrls = cleaned;
                }

                Db.Homes.Add(newHome);
                await Db.SaveChangesAsync();

                // If we stored the raw primary image, make a local URL for later display
                if (newHome.ImageBytes != null && !string.IsNullOrWhiteSpace(newHome.ImageContentType))
                {
                    newHome.ImageUrl = $"/api/homes/{newHome.Id}/image";
                    await Db.SaveChangesAsync();
                }

                Nav.NavigateTo("/homes");
            }
            catch (Exception ex)
            {
                error = $"Failed to save home: {ex.Message}";
            }
        }

        private async Task OnImageSelected(InputFileChangeEventArgs e)
        {
            error = null;
            imagePreviewDataUrl = null;
            var file = e.File;
            if (file is null)
            {
                newHome.ImageBytes = null;
                newHome.ImageContentType = null;
                return;
            }

            // Limit file size to 4MB to avoid large payloads in SignalR
            const long maxFileSize = 4 * 1024 * 1024;
            if (file.Size > maxFileSize)
            {
                error = "Image too large. Maximum size is 4 MB.";
                newHome.ImageBytes = null;
                newHome.ImageContentType = null;
                return;
            }

            try
            {
                using var stream = file.OpenReadStream(maxFileSize);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                newHome.ImageBytes = ms.ToArray();
                newHome.ImageContentType = file.ContentType;
                imagePreviewDataUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(newHome.ImageBytes)}";
            }
            catch (Exception ex)
            {
                error = $"Failed to read image: {ex.Message}";
                newHome.ImageBytes = null;
                newHome.ImageContentType = null;
            }
        }

        private async Task OnAdditionalImagesSelected(InputFileChangeEventArgs e)
        {
            error = null;
            additionalImages.Clear();
            additionalImagePreviews.Clear();

            const long maxFileSize = 4 * 1024 * 1024; // 4 MB per image
            var files = e.GetMultipleFiles(10);
            foreach (var file in files)
            {
                if (file.Size > maxFileSize)
                {
                    error = "One or more images are too large (max 4 MB each).";
                    continue;
                }

                try
                {
                    using var stream = file.OpenReadStream(maxFileSize);
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    var bytes = ms.ToArray();
                    additionalImages.Add((bytes, file.ContentType, file.Name));
                    additionalImagePreviews.Add($"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}");
                }
                catch (Exception ex)
                {
                    error = $"Failed to read an additional image: {ex.Message}";
                }
            }
        }

        private void ClearImage()
        {
            newHome.ImageBytes = null;
            newHome.ImageContentType = null;
            imagePreviewDataUrl = null;
        }

        private void ClearAdditionalImages()
        {
            additionalImages.Clear();
            additionalImagePreviews.Clear();
        }

        private void AddExternalUrl()
        {
            externalUrls.Add(new ExternalUrl());
        }

        private void RemoveExternalUrl(int index)
        {
            if (index >= 0 && index < externalUrls.Count && externalUrls.Count > 1)
            {
                externalUrls.RemoveAt(index);
            }
        }
    }
}
