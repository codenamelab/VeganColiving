using Coliving.BlazorApp.Components;
using Coliving.BlazorApp.Components.Account;
using Coliving.BlazorApp.Data;
using Coliving.BlazorApp.Models.Matrix.Core.Models;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Coliving.BlazorApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ColivingDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ColivingDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            // Localization
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Configure supported cultures (English and Norwegian Bokmål)
            var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("nb-NO") };
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                RequestCultureProviders =
                [
                    new CookieRequestCultureProvider(),
                    new AcceptLanguageHeaderRequestCultureProvider()
                ]
            };
            app.UseRequestLocalization(localizationOptions);

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            // Minimal API to serve home images stored in DB
            app.MapGet("/api/homes/{id:int}/image", async (int id, ColivingDbContext db) =>
            {
                var home = await db.Homes.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
                if (home?.ImageBytes is null || home.ImageBytes.Length == 0)
                {
                    return Results.NotFound();
                }
                var contentType = string.IsNullOrWhiteSpace(home.ImageContentType) ? "image/jpeg" : home.ImageContentType;
                return Results.File(home.ImageBytes, contentType);
            });

            // Minimal API to serve images from the Images table by id
            app.MapGet("/api/images/{id:int}", async (int id, ColivingDbContext db) =>
            {
                var image = await db.Images.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
                if (image?.Data is null || image.Data.Length == 0)
                {
                    return Results.NotFound();
                }
                var contentType = string.IsNullOrWhiteSpace(image.ContentType) ? "image/jpeg" : image.ContentType;
                var fileName = string.IsNullOrWhiteSpace(image.FileName) ? $"image-{id}.jpg" : image.FileName;
                return Results.File(image.Data, contentType, fileDownloadName: fileName);
            });

            // Endpoint to set culture via cookie and redirect back
            app.MapGet("set-culture/{culture}", (HttpContext httpContext, string culture) =>
            {
                // prefer explicit returnUrl query string; fall back to Referer; then root
                var query = httpContext.Request.Query;
                var returnUrl = query.ContainsKey("returnUrl") ? query["returnUrl"].ToString() : httpContext.Request.Headers.Referer.ToString();
                if (string.IsNullOrWhiteSpace(returnUrl))
                {
                    returnUrl = "/";
                }

                // Prevent open redirect: only allow local URLs
                static bool IsLocalUrl(string url)
                {
                    if (string.IsNullOrEmpty(url)) return false;
                    if (url[0] != '/') return false;
                    // url is local if it starts with '/' but not '//' or '/\'
                    return url.Length == 1 || (url[1] != '/' && url[1] != '\\');
                }

                if (!IsLocalUrl(returnUrl))
                {
                    returnUrl = "/";
                }

                var requestCulture = new RequestCulture(culture);
                httpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(requestCulture),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true, Path = "/" }
                );

                return Results.Redirect(returnUrl);
            });

            app.Run();
        }
    }
}
