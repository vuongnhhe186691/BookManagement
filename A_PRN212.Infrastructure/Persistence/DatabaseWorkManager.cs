using A_PRN212.Domain.Interfaces;

namespace A_PRN212.Infrastructure.Persistence
{
    /// <summary>
    /// Provides a shared UnitOfWork instance for the application lifecycle.
    /// </summary>
    public sealed class DatabaseWorkManager
    {
        private static readonly Lazy<DatabaseWorkManager> _instance = new(() => new DatabaseWorkManager());

        public static DatabaseWorkManager Instance => _instance.Value;

        public IUnitOfWork UnitOfWork { get; }

        private DatabaseWorkManager()
        {
            var context = new LibraryDbContext();
            UnitOfWork = new UnitOfWork(context);
        }
    }
}
