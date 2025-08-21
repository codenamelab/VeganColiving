using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coliving.BlazorApp.Models
{
	[Table("VeganColiving_Image")]
	public class Image
    {
        [Key]
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public byte[]? Data { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}