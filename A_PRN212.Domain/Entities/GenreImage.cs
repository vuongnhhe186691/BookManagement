using System.ComponentModel.DataAnnotations;

namespace A_PRN212.Domain.Entities
{
    public class GenreImage
    {
        [Key]
        public int ImageId { get; set; }
        public int GenreId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
