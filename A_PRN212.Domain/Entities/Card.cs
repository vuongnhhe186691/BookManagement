using System.ComponentModel.DataAnnotations;

namespace A_PRN212.Domain.Entities
{
    public class Card
    {
        [Key]
        public int CardId { get; set; }
        public int UserId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } = "Active";
    }
}
