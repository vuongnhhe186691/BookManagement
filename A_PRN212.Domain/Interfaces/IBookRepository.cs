using A_PRN212.Domain.Entities;

namespace A_PRN212.Domain.Interfaces
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        IEnumerable<Book> GetBooksWithDetails();
        IEnumerable<Book> SearchBooks(string? title, string? publisher);
    }
}
