using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Interfaces;
using A_PRN212.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace A_PRN212.Infrastructure.Repositories
{
    public class BorrowRecordRepository : GenericRepository<BorrowRecord>, IBorrowRecordRepository
    {
        public BorrowRecordRepository(LibraryDbContext context) : base(context)
        {
        }

        public IEnumerable<BorrowRecord> GetByUserId(int userId)
        {
            return _dbSet.AsNoTracking()
                .Where(br => br.UserId == userId)
                .OrderByDescending(br => br.BorrowDate)
                .ToList();
        }

        public BorrowRecord? GetLatestByUserId(int userId)
        {
            return _dbSet.AsNoTracking()
                .Where(br => br.UserId == userId)
                .OrderByDescending(br => br.BorrowDate)
                .FirstOrDefault();
        }
    }
}
