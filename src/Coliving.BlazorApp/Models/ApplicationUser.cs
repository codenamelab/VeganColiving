using Microsoft.AspNetCore.Identity;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Coliving.BlazorApp.Models
{

	namespace Matrix.Core.Models
	{
		public class ApplicationUser : IdentityUser<int>/*, IActor*/
		{
			[MaxLength(100)]
			public string FirstName { get; set; } //required

			[MaxLength(100)]
			public string LastName { get; set; } //required

			public DateTime DateOfBirth { get; set; }

			public int Points { get; set; }

			public int ReadingWordsPerMinute { get; set; }
			public int TypingCharPerMinute { get; set; }

			public string CardNumber { get; set; }
			public string SecurityNumber { get; set; }

			[RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "Expiration should match a valid MM/YY value")]
			public string Expiration { get; set; }

			public string CardHolderName { get; set; }
			public int CardType { get; set; }

			public string Street { get; set; }
			public string City { get; set; }

			public string State { get; set; }

			public string Country { get; set; }

			public string ZipCode { get; set; }
			public string Name { get; set; }

			[NotMapped]
			public int Sustainability { get; set; }

			[NotMapped]
			public int Kindness { get; set; }

			[NotMapped]
			public string Gender { get; set; } = string.Empty;

			[NotMapped]
			public int ImageId { get; set; } = 0;

			/// <summary>
			/// Indicates if the user is specific to the Coliving app.
			/// </summary>
			public bool IsColivingUser { get; set; }

			// Navigation property
			//public ICollection<UserLifeArea>? UserLifeAreas { get; set; }
			//public ICollection<Special.Task>? Tasks { get; set; }
			//public ICollection<Idea>? Ideas { get; set; }
			//public ICollection<Skill>? Skills { get; set; }

			public ApplicationUser()
			{
				var rnd = new Random();
				Sustainability = rnd.Next(1, 101); // 1 to 100 inclusive
				Kindness = rnd.Next(1, 101); // 1 to 100 inclusive
			}
		}
	}

}
