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

    protected List<Home>? homes;
    protected Dictionary<int, EngagementStatus> engagements = new();
    protected ApplicationUser? currentUser;
    protected Dictionary<int, List<ApplicationUser>> interestedUsersByHome = new();
    protected Dictionary<int, List<ApplicationUser>> committedUsersByHome = new();
    protected string? _testValue;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            homes = await ColivingDbContext.Homes
                .OrderByDescending(h => h.DateListed)
                .Take(50)
                .ToListAsync();

            if (AuthStateTask != null)
            {
                var state = await AuthStateTask;
                currentUser = await UserManager.GetUserAsync(state.User);
                if (currentUser != null && homes.Count > 0)
                {
                    var homeIds = homes.Select(f => f.Id).ToList();
                    engagements = await ColivingDbContext.HomeEngagements
                        .Where(e => e.UserId == currentUser.Id && homeIds.Contains(e.HomeId))
                        .ToDictionaryAsync(e => e.HomeId, e => e.Status);

                    var allEngagements = await ColivingDbContext.HomeEngagements
                        .Where(e => homeIds.Contains(e.HomeId))
                        .Include(e => e.User)
                        .ToListAsync();

                    interestedUsersByHome = allEngagements
                        .Where(e => e.Status == EngagementStatus.Interested && e.User != null)
                        .GroupBy(e => e.HomeId)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.User!).ToList());

                    committedUsersByHome = allEngagements
                        .Where(e => e.Status == EngagementStatus.Committed && e.User != null)
                        .GroupBy(e => e.HomeId)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.User!).ToList());
                }
            }
        }
        catch
        {
            homes = new List<Home>();
        }
    }

    protected string? GetHomeImageUrl(Home home)
        => !string.IsNullOrWhiteSpace(home.ImageUrl)
            ? home.ImageUrl
            : (home.ImageBytes != null && home.ImageBytes.Length > 0
                ? $"/api/homes/{home.Id}/image"
                : null);

    protected bool IsStatus(int homeId, EngagementStatus status) => engagements.TryGetValue(homeId, out var s) && s == status;
    protected bool IsStatusSet(int homeId) => engagements.TryGetValue(homeId, out var s) && s != EngagementStatus.None;

    protected string GetInterestedBtnClass(int homeId) => IsStatus(homeId, EngagementStatus.Interested) ? "btn-success" : "btn-outline-success";
    protected string GetCommittedBtnClass(int homeId) => IsStatus(homeId, EngagementStatus.Committed) ? "btn-primary" : "btn-outline-primary";

    protected async Task SetEngagement(int homeId, EngagementStatus status)
    {
        if (currentUser == null) return;

        var existing = await ColivingDbContext.HomeEngagements.FindAsync(currentUser.Id, homeId);
        if (existing == null)
        {
            ColivingDbContext.HomeEngagements.Add(new HomeEngagement
            {
                UserId = currentUser.Id,
                HomeId = homeId,
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
        engagements[homeId] = status;
        RemoveFromLists(homeId, currentUser);
        AddToList(homeId, currentUser, status);
        StateHasChanged();
    }

    protected async Task ClearEngagement(int homeId)
    {
        if (currentUser == null) return;
        var existing = await ColivingDbContext.HomeEngagements.FindAsync(currentUser.Id, homeId);
        if (existing != null)
        {
            ColivingDbContext.HomeEngagements.Remove(existing);
            await ColivingDbContext.SaveChangesAsync();
        }
        engagements.Remove(homeId);
        RemoveFromLists(homeId, currentUser);
        StateHasChanged();
    }

    protected void RemoveFromLists(int homeId, ApplicationUser user)
    {
        if (interestedUsersByHome.TryGetValue(homeId, out var intList))
        {
            intList.RemoveAll(u => u.Id == user.Id);
            if (intList.Count == 0) interestedUsersByHome.Remove(homeId);
        }
        if (committedUsersByHome.TryGetValue(homeId, out var comList))
        {
            comList.RemoveAll(u => u.Id == user.Id);
            if (comList.Count == 0) committedUsersByHome.Remove(homeId);
        }
    }

    protected void AddToList(int homeId, ApplicationUser user, EngagementStatus status)
    {
        var dict = status == EngagementStatus.Interested ? interestedUsersByHome : committedUsersByHome;
        if (!dict.TryGetValue(homeId, out var list))
        {
            list = new List<ApplicationUser>();
            dict[homeId] = list;
        }
        if (!list.Any(u => u.Id == user.Id))
        {
            list.Add(user);
        }
    }

    protected RenderFragment RenderUserList(int homeId, Dictionary<int, List<ApplicationUser>> dict) => builder =>
    {
        const int maxNames = 3;
        if (!dict.TryGetValue(homeId, out var users) || users.Count == 0)
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
