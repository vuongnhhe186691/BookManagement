using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Interfaces;
using A_PRN212.Infrastructure.Persistence;

namespace A_PRN212.Infrastructure.Repositories
{
    public class LibraryRuleRepository : GenericRepository<LibraryRule>, ILibraryRuleRepository
    {
        public LibraryRuleRepository(LibraryDbContext context) : base(context)
        {
        }
    }
}
