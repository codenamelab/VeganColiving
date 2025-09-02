using Coliving.BlazorApp.Models;
using Coliving.BlazorApp.Models.Matrix.Core.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Coliving.BlazorApp.Data
{
	//public class ColivingDbContext(DbContextOptions<ColivingDbContext> options) : IdentityDbContext<ColivingDbContext>(options)
	public class ColivingDbContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
	{
		public ColivingDbContext(DbContextOptions<ColivingDbContext> options) : base(options)
		{

		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// Only configure if not already configured (for cases where options are not provided)
			if (!optionsBuilder.IsConfigured)
			{
				//optionsBuilder.UseSqlServer("Server=localhost;Database=matrix3;User Id=architect;Password=Jippi,123;TrustServerCertificate=true;");
				_ = optionsBuilder.UseSqlServer("Server=tcp:matrixn.database.windows.net,1433;Initial Catalog=vegancoliving1;Persist Security Info=False;User ID=architect;Password=Jippi,123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Precision for decimal fields
			modelBuilder.Entity<Home>()
				.Property(f => f.PricePerMonth)
				.HasPrecision(18, 2);

			modelBuilder.Entity<Room>()
				.Property(r => r.PricePerMonth)
				.HasPrecision(18, 2);

			// Precision for ApplicationUser rental preferences
			modelBuilder.Entity<ApplicationUser>(entity =>
			{
				entity.Property(u => u.MinMonthlyRentalPrice).HasPrecision(18, 0);
				entity.Property(u => u.MaxMonthlyRentalPrice).HasPrecision(18, 0);
			});

			// Relationships: Home has many Rooms (legacy column name HomeId preserved via [Column])
			modelBuilder.Entity<Room>()
				.HasOne(r => r.Home)
				.WithMany(f => f.Rooms!)
				.HasForeignKey(r => r.HomeId)
				.OnDelete(DeleteBehavior.Cascade);

			// HomeEngagement composite key and relationships (legacy column name HoomeId preserved via [Column])
			modelBuilder.Entity<HomeEngagement>(entity =>
			{
				entity.HasKey(e => new { e.UserId, e.HomeId });
				entity.HasOne(e => e.User)
					  .WithMany()
					  .HasForeignKey(e => e.UserId)
					  .OnDelete(DeleteBehavior.Cascade);
				entity.HasOne(e => e.Home)
					  .WithMany()
					  .HasForeignKey(e => e.HomeId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// Home has many Images
			modelBuilder.Entity<Home>()
				.HasMany(h => h.Images)
				.WithOne()
				.OnDelete(DeleteBehavior.Cascade);

			// Home has many ExternalUrls (legacy column name handled via attribute on model)
			modelBuilder.Entity<Home>()
				.HasMany(h => h.ExternalUrls)
				.WithOne(e => e.Home!)
				.HasForeignKey(e => e.HomeId)
				.OnDelete(DeleteBehavior.Cascade);
		}

	public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
	public DbSet<Home> Homes { get; set; } = null!;
	public DbSet<HomeEngagement> HomeEngagements { get; set; } = null!; // renamed from HomeEngagements
	public DbSet<Room> Rooms { get; set; } = null!;
	public DbSet<Image> Images { get; set; } = null!;
	public DbSet<ExternalUrl> ExternalUrls { get; set; } = null!; // renamed from HomeExternalUrls

	}
}
