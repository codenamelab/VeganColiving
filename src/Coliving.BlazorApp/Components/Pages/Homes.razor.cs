using Coliving.BlazorApp.Data;
using Coliving.BlazorApp.Models;
using Coliving.BlazorApp.Models.Matrix.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Coliving.BlazorApp.Components.Pages;

public partial class Homes : ComponentBase
{
    [Inject] protected ColivingDbContext ColivingDbContext { get; set; } = default!;
    [Inject] protected UserManager<ApplicationUser> UserManager { get; set; } = default!;
    [Inject] protected Microsoft.Extensions.Localization.IStringLocalizerFactory LocFactory { get; set; } = default!;
    [CascadingParameter] protected Task<AuthenticationState>? AuthStateTask { get; set; }

    protected List<Flat>? flats;
    protected Dictionary<int, EngagementStatus> engagements = new();
    protected ApplicationUser? currentUser;
    protected Dictionary<int, List<ApplicationUser>> interestedUsersByFlat = new();
    protected Dictionary<int, List<ApplicationUser>> committedUsersByFlat = new();
    protected string? _testValue;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            flats = await ColivingDbContext.Flats
                .OrderByDescending(h => h.DateListed)
                .Take(50)
                .ToListAsync();

            if (AuthStateTask != null)
            {
                var state = await AuthStateTask;
                currentUser = await UserManager.GetUserAsync(state.User);
                if (currentUser != null && flats.Count > 0)
                {
                    var flatIds = flats.Select(f => f.Id).ToList();
                    engagements = await ColivingDbContext.FlatEngagements
                        .Where(e => e.UserId == currentUser.Id && flatIds.Contains(e.FlatId))
                        .ToDictionaryAsync(e => e.FlatId, e => e.Status);

                    var allEngagements = await ColivingDbContext.FlatEngagements
                        .Where(e => flatIds.Contains(e.FlatId))
                        .Include(e => e.User)
                        .ToListAsync();

                    interestedUsersByFlat = allEngagements
                        .Where(e => e.Status == EngagementStatus.Interested && e.User != null)
                        .GroupBy(e => e.FlatId)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.User!).ToList());

                    committedUsersByFlat = allEngagements
                        .Where(e => e.Status == EngagementStatus.Committed && e.User != null)
                        .GroupBy(e => e.FlatId)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.User!).ToList());
                }
            }
        }
        catch
        {
            flats = new List<Flat>();
        }
    }

    protected string? GetFlatImageUrl(Flat flat)
        => !string.IsNullOrWhiteSpace(flat.ImageUrl)
            ? flat.ImageUrl
            : (flat.ImageBytes != null && flat.ImageBytes.Length > 0
                ? $"/api/flats/{flat.Id}/image"
                : null);

    protected bool IsStatus(int flatId, EngagementStatus status) => engagements.TryGetValue(flatId, out var s) && s == status;
    protected bool IsStatusSet(int flatId) => engagements.TryGetValue(flatId, out var s) && s != EngagementStatus.None;

    protected string GetInterestedBtnClass(int flatId) => IsStatus(flatId, EngagementStatus.Interested) ? "btn-success" : "btn-outline-success";
    protected string GetCommittedBtnClass(int flatId) => IsStatus(flatId, EngagementStatus.Committed) ? "btn-primary" : "btn-outline-primary";

    protected async Task SetEngagement(int flatId, EngagementStatus status)
    {
        if (currentUser == null) return;

        var existing = await ColivingDbContext.FlatEngagements.FindAsync(currentUser.Id, flatId);
        if (existing == null)
        {
            ColivingDbContext.FlatEngagements.Add(new FlatEngagement
            {
                UserId = currentUser.Id,
                FlatId = flatId,
                Status = status,
                UpdatedUtc = DateTime.UtcNow
            });
        }
        else
        {
            existing.Status = status;
            existing.UpdatedUtc = DateTime.UtcNow;
        }

        await ColivingDbContext.SaveChangesAsync();
        engagements[flatId] = status;
        RemoveFromLists(flatId, currentUser);
        AddToList(flatId, currentUser, status);
        StateHasChanged();
    }

    protected async Task ClearEngagement(int flatId)
    {
        if (currentUser == null) return;
        var existing = await ColivingDbContext.FlatEngagements.FindAsync(currentUser.Id, flatId);
        if (existing != null)
        {
            ColivingDbContext.FlatEngagements.Remove(existing);
            await ColivingDbContext.SaveChangesAsync();
        }
        engagements.Remove(flatId);
        RemoveFromLists(flatId, currentUser);
        StateHasChanged();
    }

    protected void RemoveFromLists(int flatId, ApplicationUser user)
    {
        if (interestedUsersByFlat.TryGetValue(flatId, out var intList))
        {
            intList.RemoveAll(u => u.Id == user.Id);
            if (intList.Count == 0) interestedUsersByFlat.Remove(flatId);
        }
        if (committedUsersByFlat.TryGetValue(flatId, out var comList))
        {
            comList.RemoveAll(u => u.Id == user.Id);
            if (comList.Count == 0) committedUsersByFlat.Remove(flatId);
        }
    }

    protected void AddToList(int flatId, ApplicationUser user, EngagementStatus status)
    {
        var dict = status == EngagementStatus.Interested ? interestedUsersByFlat : committedUsersByFlat;
        if (!dict.TryGetValue(flatId, out var list))
        {
            list = new List<ApplicationUser>();
            dict[flatId] = list;
        }
        if (!list.Any(u => u.Id == user.Id))
        {
            list.Add(user);
        }
    }

    protected RenderFragment RenderUserList(int flatId, Dictionary<int, List<ApplicationUser>> dict) => builder =>
    {
        const int maxNames = 3;
        if (!dict.TryGetValue(flatId, out var users) || users.Count == 0)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "text-muted small");
            builder.AddContent(2, "No one yet");
            builder.CloseElement();
            return;
        }

        var display = users.Take(maxNames).Select(FormatUserName).ToList();
        var more = users.Count - display.Count;

        builder.OpenElement(3, "div");
        builder.AddAttribute(4, "class", "small");
        builder.AddContent(5, string.Join(", ", display));
        if (more > 0)
        {
            builder.AddContent(6, $", +{more} more");
        }
        builder.CloseElement();
    };

    protected string FormatUserName(ApplicationUser u)
    {
        if (!string.IsNullOrWhiteSpace(u.FirstName) || !string.IsNullOrWhiteSpace(u.LastName))
        {
            return $"{u.FirstName} {u.LastName}".Trim();
        }
        if (!string.IsNullOrWhiteSpace(u.Name)) return u.Name!;
        if (!string.IsNullOrWhiteSpace(u.Email)) return u.Email!.Split('@')[0];
        return $"User {u.Id}";
    }
}
