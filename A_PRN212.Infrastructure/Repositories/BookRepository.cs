using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Interfaces;
using A_PRN212.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace A_PRN212.Infrastructure.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(LibraryDbContext context) : base(context)
        {
        }

        public IEnumerable<Book> GetBooksWithDetails()
        {
            return _dbSet
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Book> SearchBooks(string? title, string? publisher)
        {
            var query = _dbSet
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                string trimmedTitle = title.Trim();
                query = query.Where(b => b.Title.Contains(trimmedTitle));
            }

            if (!string.IsNullOrWhiteSpace(publisher))
            {
                string trimmedPublisher = publisher.Trim();
                query = query.Where(b => b.Publisher != null && b.Publisher.Contains(trimmedPublisher));
            }

            return query.ToList();
        }
    }
}
