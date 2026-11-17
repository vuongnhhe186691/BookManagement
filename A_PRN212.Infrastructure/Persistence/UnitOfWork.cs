using A_PRN212.Domain.Interfaces;
using A_PRN212.Infrastructure.Repositories;

namespace A_PRN212.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryDbContext _context;

        private IBookRepository? _books;
        private IAuthorRepository? _authors;
        private IGenreRepository? _genres;
        private IUserRepository? _users;
        private IBorrowRecordRepository? _borrowRecords;
        private ILibraryRuleRepository? _libraryRules;
        private ICardRepository? _cards;

        public UnitOfWork(LibraryDbContext context)
        {
            _context = context;
        }

        public IBookRepository Books => _books ??= new BookRepository(_context);
        public IAuthorRepository Authors => _authors ??= new AuthorRepository(_context);
        public IGenreRepository Genres => _genres ??= new GenreRepository(_context);
        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IBorrowRecordRepository BorrowRecords => _borrowRecords ??= new BorrowRecordRepository(_context);
        public ILibraryRuleRepository LibraryRules => _libraryRules ??= new LibraryRuleRepository(_context);
        public ICardRepository Cards => _cards ??= new CardRepository(_context);

        public int SaveChanges()
        {
            try
            {
                return _context.SaveChanges();
            }
            finally
            {
                _context.ChangeTracker.Clear();
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
