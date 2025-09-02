using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Coliving.BlazorApp.Models;
using Coliving.BlazorApp.Data;
using Microsoft.AspNetCore.Components.Forms;

namespace Coliving.BlazorApp.Components.Pages;

public partial class EditHome : ComponentBase
{
    [Parameter]
    public int id { get; set; }

    [Inject] private ColivingDbContext Db { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private Home? existingHome;
    private string? error;
    private string? imagePreviewDataUrl;
    private string? originalImageUrl;
    private readonly List<(byte[] bytes, string contentType, string? title)> additionalImages = new();
    private readonly List<string> additionalImagePreviews = new();
    private bool showDeleteConfirm;

    // External URLs editing
    private List<ExternalUrl> externalUrls = new();

    protected override async Task OnParametersSetAsync()
    {
        try
        {
            existingHome = await Db.Homes
                .Include(f => f.Images)
                .Include(f => f.ExternalUrls)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (existingHome is null)
            {
                error = "Home not found.";
                return;
            }

            // Clone external URLs into an editable list (avoid directly binding tracked entities for removed items)
            externalUrls = existingHome.ExternalUrls?.Select(u => new ExternalUrl
            {
                Id = u.Id,
                HomeId = u.HomeId,
                Source = u.Source,
                Url = u.Url,
                Notes = u.Notes,
                CreatedUtc = u.CreatedUtc
            }).ToList() ?? new List<ExternalUrl>();
            if (externalUrls.Count == 0)
                externalUrls.Add(new ExternalUrl { HomeId = existingHome.Id });

            // Keep a reference to the current image shown
            originalImageUrl = !string.IsNullOrWhiteSpace(existingHome.ImageUrl)
                ? existingHome.ImageUrl
                : (existingHome.ImageBytes != null && existingHome.ImageBytes.Length > 0 ? $"/api/homes/{existingHome.Id}/image" : null);
        }
        catch (Exception ex)
        {
            error = $"Failed to load home: {ex.Message}";
        }
    }

    private async Task HandleValidSubmit()
    {
        try
        {
            // Primary image: ensure URL when raw bytes present
            if (existingHome!.ImageBytes != null && !string.IsNullOrWhiteSpace(existingHome.ImageContentType))
            {
                existingHome.ImageUrl = $"/api/homes/{existingHome.Id}/image";
            }

            // Additional images
            if (additionalImages.Count > 0)
            {
                existingHome.Images ??= new List<Image>();
                foreach (var (bytes, contentType, title) in additionalImages.Take(10))
                {
                    existingHome.Images.Add(new Image
                    {
                        Data = bytes,
                        ContentType = contentType,
                        Title = title,
                        FileName = null
                    });
                }
            }

            // External URLs sync
            var validUrls = externalUrls
                .Where(u => !string.IsNullOrWhiteSpace(u.Source) && !string.IsNullOrWhiteSpace(u.Url))
                .ToList();

            existingHome.ExternalUrls ??= new List<ExternalUrl>();
            var tracked = existingHome.ExternalUrls.ToList();

            // Remove deleted
            foreach (var old in tracked.Where(o => !validUrls.Any(v => v.Id == o.Id)))
            {
                Db.ExternalUrls.Remove(old);
            }

            // Add or update
            foreach (var v in validUrls)
            {
                if (v.Id == 0)
                {
                    v.HomeId = existingHome.Id;
                    existingHome.ExternalUrls.Add(v); // attach new
                }
                else
                {
                    var match = tracked.First(o => o.Id == v.Id);
                    match.Source = v.Source;
                    match.Url = v.Url;
                    match.Notes = v.Notes;
                }
            }

            await Db.SaveChangesAsync();
            Nav.NavigateTo($"/homes/{existingHome!.Id}");
        }
        catch (Exception ex)
        {
            error = $"Failed to save changes: {ex.Message}";
        }
    }

    private async Task DeleteHomeAsync()
    {
        if (existingHome is null) return;

        try
        {
            Db.Homes.Remove(existingHome);
            await Db.SaveChangesAsync();
            Nav.NavigateTo("/homes");
        }
        catch (Exception ex)
        {
            error = $"Failed to delete home: {ex.Message}";
            showDeleteConfirm = false;
        }
    }

    private async Task OnImageSelected(InputFileChangeEventArgs e)
    {
        error = null;
        imagePreviewDataUrl = null;
        var file = e.File;
        if (file is null || existingHome is null)
        {
            if (existingHome is not null)
            {
                existingHome.ImageBytes = null;
                existingHome.ImageContentType = null;
            }
            return;
        }

        const long maxFileSize = 4 * 1024 * 1024; // 4 MB
        if (file.Size > maxFileSize)
        {
            error = "Image too large. Maximum size is 4 MB.";
            existingHome.ImageBytes = null;
            existingHome.ImageContentType = null;
            return;
        }

        try
        {
            using var stream = file.OpenReadStream(maxFileSize);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            existingHome.ImageBytes = ms.ToArray();
            existingHome.ImageContentType = file.ContentType;
            imagePreviewDataUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(existingHome.ImageBytes)}";
        }
        catch (Exception ex)
        {
            error = $"Failed to read image: {ex.Message}";
            existingHome.ImageBytes = null;
            existingHome.ImageContentType = null;
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
        if (existingHome is null) return;
        existingHome.ImageBytes = null;
        existingHome.ImageContentType = null;
        imagePreviewDataUrl = null;
    }

    private void ClearAdditionalImages()
    {
        additionalImages.Clear();
        additionalImagePreviews.Clear();
    }

    private void AddExternalUrl()
    {
        externalUrls.Add(new ExternalUrl { HomeId = existingHome?.Id ?? 0 });
    }

    private void RemoveExternalUrl(int index)
    {
        if (index >= 0 && index < externalUrls.Count && externalUrls.Count > 1)
        {
            externalUrls.RemoveAt(index);
        }
    }
}
