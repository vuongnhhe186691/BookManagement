using A_PRN212.Application.Interfaces;
using A_PRN212.Application.Services;
using A_PRN212.Domain.Interfaces;
using A_PRN212.Infrastructure.Email;
using A_PRN212.Infrastructure.Persistence;

namespace A_PRN212.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Wires and exposes application services with their infrastructure dependencies.
    /// </summary>
    public sealed class ServiceProviderFactory
    {
        private static readonly Lazy<ServiceProviderFactory> _instance = new(() => new ServiceProviderFactory());

        public static ServiceProviderFactory Instance => _instance.Value;

        public IUnitOfWork UnitOfWork { get; }
        public IEmailService EmailService { get; }
        public IAuthService AuthService { get; }
        public IBookService BookService { get; }
        public IAuthorService AuthorService { get; }
        public IGenreService GenreService { get; }
        public IUserService UserService { get; }
        public IBorrowService BorrowService { get; }
        public ILibraryRuleService LibraryRuleService { get; }

        private ServiceProviderFactory()
        {
            UnitOfWork = DatabaseWorkManager.Instance.UnitOfWork;
            EmailService = new SmtpEmailService();

            AuthService = new AuthService(UnitOfWork, EmailService);
            BookService = new BookService(UnitOfWork);
            AuthorService = new AuthorService(UnitOfWork);
            GenreService = new GenreService(UnitOfWork);
            UserService = new UserService(UnitOfWork);
            BorrowService = new BorrowService(UnitOfWork);
            LibraryRuleService = new LibraryRuleService(UnitOfWork);

            try
            {
                UserService.EnsureAllPasswordsHashed();
            }
            catch
            {
                // Allow startup even if SQL Server is unreachable until user connects
            }
        }
    }
}
