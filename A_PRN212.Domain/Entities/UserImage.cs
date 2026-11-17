using System.ComponentModel.DataAnnotations;

namespace A_PRN212.Domain.Entities
{
    public class UserImage
    {
        [Key]
        public int ImageId { get; set; }
        public int UserId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
