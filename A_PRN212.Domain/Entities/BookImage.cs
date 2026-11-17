using System.ComponentModel.DataAnnotations;

namespace A_PRN212.Domain.Entities
{
    public class BookImage
    {
        [Key]
        public int ImageId { get; set; }
        public int BookId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
