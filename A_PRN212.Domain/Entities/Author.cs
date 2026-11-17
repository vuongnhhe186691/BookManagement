namespace A_PRN212.Domain.Entities
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string? Biography { get; set; }
        public string? Nationality { get; set; }
        public DateTime? BirthDate { get; set; }
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
