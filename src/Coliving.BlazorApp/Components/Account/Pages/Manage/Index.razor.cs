using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Coliving.BlazorApp.Models.Matrix.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Coliving.BlazorApp.Data;

namespace Coliving.BlazorApp.Components.Account.Pages.Manage
{
    public partial class Index : ComponentBase
    {
        [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = default!;
        [Inject] private SignInManager<ApplicationUser> SignInManager { get; set; } = default!;
        [Inject] private IdentityUserAccessor UserAccessor { get; set; } = default!;
        [Inject] private IdentityRedirectManager RedirectManager { get; set; } = default!;

        private ApplicationUser user = default!;
        private string? username;
        private string? phoneNumber;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            username = await UserManager.GetUserNameAsync(user);
            phoneNumber = await UserManager.GetPhoneNumberAsync(user);

            // Map existing user values into the input model
            Input.PhoneNumber ??= phoneNumber;
            Input.FirstName = user.FirstName;
            Input.LastName = user.LastName;
            Input.DateOfBirth = user.DateOfBirth;
            Input.Points = user.Points;
            Input.CardNumber = user.CardNumber;
            Input.SecurityNumber = user.SecurityNumber;
            Input.Expiration = user.Expiration;
            Input.CardHolderName = user.CardHolderName;
            Input.CardType = user.CardType;
            Input.Street = user.Street;
            Input.City = user.City;
            Input.State = user.State;
            Input.Country = user.Country;
            Input.ZipCode = user.ZipCode;
            Input.Name = user.Name;
            Input.IsColivingUser = user.IsColivingUser;
        }

        private async Task OnValidSubmitAsync()
        {
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await UserManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    RedirectManager.RedirectToCurrentPageWithStatus("Error: Failed to set phone number.", HttpContext);
                }
            }

            // Update other profile fields
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.DateOfBirth = Input.DateOfBirth;
            user.Points = Input.Points;
            user.CardNumber = Input.CardNumber;
            user.SecurityNumber = Input.SecurityNumber;
            user.Expiration = Input.Expiration;
            user.CardHolderName = Input.CardHolderName;
            user.CardType = Input.CardType;
            user.Street = Input.Street;
            user.City = Input.City;
            user.State = Input.State;
            user.Country = Input.Country;
            user.ZipCode = Input.ZipCode;
            user.Name = Input.Name;
            user.IsColivingUser = Input.IsColivingUser;

            var updateResult = await UserManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                RedirectManager.RedirectToCurrentPageWithStatus("Error: Failed to update profile.", HttpContext);
                return;
            }

            await SignInManager.RefreshSignInAsync(user);
            RedirectManager.RedirectToCurrentPageWithStatus("Your profile has been updated", HttpContext);
        }

        private sealed class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }

            [MaxLength(100)]
            [Display(Name = "First name")]
            public string? FirstName { get; set; }

            [MaxLength(100)]
            [Display(Name = "Last name")]
            public string? LastName { get; set; }

            [Display(Name = "Date of birth")]
            public DateTime? DateOfBirth { get; set; }

            public int? Points { get; set; }

            [Display(Name = "Card number")]
            public string? CardNumber { get; set; }

            [Display(Name = "Security number")]
            public string? SecurityNumber { get; set; }

            [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "Expiration should match a valid MM/YY value")]
            [Display(Name = "Expiration (MM/YY)")]
            public string? Expiration { get; set; }

            [Display(Name = "Card holder name")]
            public string? CardHolderName { get; set; }

            [Display(Name = "Card type")]
            public int? CardType { get; set; }

            public string? Street { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? Country { get; set; }
            [Display(Name = "Zip code")]
            public string? ZipCode { get; set; }

            [Display(Name = "Display name")]
            public string? Name { get; set; }

            [Display(Name = "Is Coliving user")]
            public bool IsColivingUser { get; set; }
        }
    }
}
