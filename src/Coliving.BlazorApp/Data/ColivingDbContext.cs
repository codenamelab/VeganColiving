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
			modelBuilder.Entity<Flat>()
				.Property(f => f.PricePerMonth)
				.HasPrecision(18, 2);

			modelBuilder.Entity<Room>()
				.Property(r => r.PricePerMonth)
				.HasPrecision(18, 2);

			// Relationships: Flat has many Rooms
			modelBuilder.Entity<Room>()
				.HasOne(r => r.Flat)
				.WithMany(f => f.Rooms!)
				.HasForeignKey(r => r.FlatId)
				.OnDelete(DeleteBehavior.Cascade);

			// FlatEngagement composite key and relationships
			modelBuilder.Entity<FlatEngagement>(entity =>
			{
				entity.HasKey(e => new { e.UserId, e.FlatId });
				entity.HasOne(e => e.User)
					  .WithMany()
					  .HasForeignKey(e => e.UserId)
					  .OnDelete(DeleteBehavior.Cascade);
				entity.HasOne(e => e.Flat)
					  .WithMany()
					  .HasForeignKey(e => e.FlatId)
					  .OnDelete(DeleteBehavior.Cascade);
			});
		}

	public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
	public DbSet<Flat> Flats { get; set; } = null!;
	public DbSet<FlatEngagement> FlatEngagements { get; set; } = null!;
	public DbSet<Room> Rooms { get; set; } = null!;

	}
}
