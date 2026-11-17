using A_PRN212.Application.Common;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Enums;
using A_PRN212.Domain.Interfaces;

namespace A_PRN212.Application.Services
{
    public class BorrowService : IBorrowService
    {
        private const decimal DailyFineVnd = 10000m;
        private const int DefaultBorrowDays = 14;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HashSet<int> _settledFineBorrowIds = new();

        public BorrowService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BorrowRecord> GetAllBorrowRecords()
        {
            return _unitOfWork.BorrowRecords.GetAll().Select(ApplyFine).ToList();
        }

        public IEnumerable<BorrowRecord> GetBorrowRecordsByUser(int userId)
        {
            return _unitOfWork.BorrowRecords.GetByUserId(userId).Select(ApplyFine).ToList();
        }

        public BorrowRecord? GetLatestBorrowRecordByUser(int userId)
        {
            var record = _unitOfWork.BorrowRecords.GetLatestByUserId(userId);
            return record != null ? ApplyFine(record) : null;
        }

        public ServiceResult<BorrowRecord> BorrowBook(int userId, int bookId)
        {
            var user = _unitOfWork.Users.GetById(userId);
            if (user == null)
            {
                return ServiceResult<BorrowRecord>.Failure("User not found.");
            }

            var book = _unitOfWork.Books.GetById(bookId);
            if (book == null)
            {
                return ServiceResult<BorrowRecord>.Failure("Book not found.");
            }

            if (book.Available <= 0)
            {
                return ServiceResult<BorrowRecord>.Failure("This book is not available for borrowing.");
            }

            DateTime borrowDate = DateTime.Now;
            DateTime dueDate = borrowDate.AddDays(DefaultBorrowDays);

            var record = new BorrowRecord
            {
                UserId = userId,
                BookId = bookId,
                BorrowDate = borrowDate,
                DueDate = dueDate,
                Status = BorrowStatus.Borrowed
            };

            _unitOfWork.BorrowRecords.Add(record);

            book.Available -= 1;
            _unitOfWork.Books.Update(book);

            _unitOfWork.SaveChanges();
            return ServiceResult<BorrowRecord>.Success(record, "Book borrowed successfully!");
        }

        public ServiceResult<BorrowRecord> ReturnBook(int borrowId)
        {
            var record = _unitOfWork.BorrowRecords.GetById(borrowId);
            if (record == null)
            {
                return ServiceResult<BorrowRecord>.Failure("Borrow record not found.");
            }

            if (record.Status == BorrowStatus.Returned || record.ReturnDate.HasValue)
            {
                return ServiceResult<BorrowRecord>.Failure("This book has already been returned.");
            }

            DateTime returnDate = DateTime.Now;
            record.ReturnDate = returnDate;

            if (returnDate <= record.DueDate)
            {
                record.Status = BorrowStatus.Returned;
                record.FineAmount = 0;
            }
            else
            {
                record.Status = BorrowStatus.Overdue;
                int overdueDays = (returnDate - record.DueDate).Days;
                record.FineAmount = overdueDays > 0 ? overdueDays * DailyFineVnd : 0m;
            }

            _unitOfWork.BorrowRecords.Update(record);

            var book = _unitOfWork.Books.GetById(record.BookId);
            if (book != null)
            {
                book.Available = Math.Min(book.Quantity, book.Available + 1);
                _unitOfWork.Books.Update(book);
            }

            _unitOfWork.SaveChanges();
            return ServiceResult<BorrowRecord>.Success(record, $"Return date updated successfully! Fine: {record.FineAmount:C}");
        }

        public ServiceResult PayFine(int borrowId)
        {
            var record = _unitOfWork.BorrowRecords.GetById(borrowId);
            if (record == null)
            {
                return ServiceResult.Failure("Borrow record not found.");
            }

            _settledFineBorrowIds.Add(borrowId);
            if (record.Status == BorrowStatus.Overdue && record.ReturnDate.HasValue)
            {
                record.Status = BorrowStatus.Returned;
                _unitOfWork.BorrowRecords.Update(record);
                _unitOfWork.SaveChanges();
            }

            return ServiceResult.Success("Payment successful.");
        }

        private BorrowRecord ApplyFine(BorrowRecord record)
        {
            if (_settledFineBorrowIds.Contains(record.BorrowId))
            {
                record.FineAmount = 0m;
                return record;
            }

            DateTime effectiveEnd = record.ReturnDate ?? DateTime.Now;
            if (record.Status == BorrowStatus.Overdue || (!record.ReturnDate.HasValue && effectiveEnd > record.DueDate))
            {
                int overdueDays = (effectiveEnd - record.DueDate).Days;
                record.FineAmount = overdueDays > 0 ? overdueDays * DailyFineVnd : 0m;
            }
            else
            {
                record.FineAmount = 0m;
            }

            return record;
        }
    }
}
