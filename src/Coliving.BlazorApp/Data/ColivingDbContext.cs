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
				_ = optionsBuilder.UseSqlServer("Server=tcp:matrixn.database.windows.net,1433;Initial Catalog=matrix3;Persist Security Info=False;User ID=architect;Password=Jippi,123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
			}
		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
		public DbSet<Flat> Flats { get; set; } = null!;

	}
}
