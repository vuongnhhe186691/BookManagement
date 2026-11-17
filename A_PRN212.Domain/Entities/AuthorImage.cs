using System.ComponentModel.DataAnnotations;

namespace A_PRN212.Domain.Entities
{
    public class AuthorImage
    {
        [Key]
        public int ImageId { get; set; }
        public int AuthorId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
