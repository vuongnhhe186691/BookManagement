namespace A_PRN212.Application.DTOs
{
    public class BookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string? Publisher { get; set; }
        public int GenreId { get; set; }
        public string GenreName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Available { get; set; }
    }
}
