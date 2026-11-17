namespace A_PRN212.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository Books { get; }
        IAuthorRepository Authors { get; }
        IGenreRepository Genres { get; }
        IUserRepository Users { get; }
        IBorrowRecordRepository BorrowRecords { get; }
        ILibraryRuleRepository LibraryRules { get; }
        ICardRepository Cards { get; }

        int SaveChanges();
    }
}
