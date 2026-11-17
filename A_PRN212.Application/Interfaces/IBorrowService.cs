using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;

namespace A_PRN212.Application.Interfaces
{
    public interface IBorrowService
    {
        IEnumerable<BorrowRecord> GetAllBorrowRecords();
        IEnumerable<BorrowRecord> GetBorrowRecordsByUser(int userId);
        BorrowRecord? GetLatestBorrowRecordByUser(int userId);
        ServiceResult<BorrowRecord> BorrowBook(int userId, int bookId);
        ServiceResult<BorrowRecord> ReturnBook(int borrowId);
        ServiceResult PayFine(int borrowId);
    }
}
