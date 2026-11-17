using A_PRN212.Application.Common;
using A_PRN212.Application.DTOs;

namespace A_PRN212.Application.Interfaces
{
    public interface IBookService
    {
        IEnumerable<BookDto> GetAllBooks();
        IEnumerable<BookDto> SearchBooks(string? title, string? publisher);
        ServiceResult AddBook(string title, string authorName, string? publisher, string genreName, int quantity, int available);
        ServiceResult UpdateBook(int bookId, string title, string authorName, string? publisher, string genreName, int quantity, int available);
        ServiceResult DeleteBook(int bookId);
    }
}
