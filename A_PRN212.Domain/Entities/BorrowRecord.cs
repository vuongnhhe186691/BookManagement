using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A_PRN212.Domain.Entities
{
    public class BorrowRecord
    {
        [Key]
        public int BorrowId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = "Not Borrowed";

        [NotMapped]
        public decimal FineAmount { get; set; }

        public virtual User? User { get; set; }
        public virtual Book? Book { get; set; }
    }
}
