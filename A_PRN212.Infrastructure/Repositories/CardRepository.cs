using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Interfaces;
using A_PRN212.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace A_PRN212.Infrastructure.Repositories
{
    public class CardRepository : GenericRepository<Card>, ICardRepository
    {
        public CardRepository(LibraryDbContext context) : base(context)
        {
        }

        public IEnumerable<Card> GetByUserId(int userId)
        {
            return _dbSet.AsNoTracking().Where(c => c.UserId == userId).ToList();
        }
    }
}
