namespace A_PRN212.Domain.Entities
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public string? Publisher { get; set; }
        public int GenreId { get; set; }
        public int Quantity { get; set; }
        public int Available { get; set; }

        public virtual Author? Author { get; set; }
        public virtual Genre? Genre { get; set; }
    }
}
