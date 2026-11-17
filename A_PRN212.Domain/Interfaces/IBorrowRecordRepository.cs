using A_PRN212.Domain.Entities;

namespace A_PRN212.Domain.Interfaces
{
    public interface IBorrowRecordRepository : IGenericRepository<BorrowRecord>
    {
        IEnumerable<BorrowRecord> GetByUserId(int userId);
        BorrowRecord? GetLatestByUserId(int userId);
    }
}
