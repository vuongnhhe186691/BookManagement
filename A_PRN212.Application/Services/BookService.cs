using A_PRN212.Application.Common;
using A_PRN212.Application.DTOs;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Interfaces;

namespace A_PRN212.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BookDto> GetAllBooks()
        {
            return _unitOfWork.Books.GetBooksWithDetails().Select(MapToDto).ToList();
        }

        public IEnumerable<BookDto> SearchBooks(string? title, string? publisher)
        {
            return _unitOfWork.Books.SearchBooks(title, publisher).Select(MapToDto).ToList();
        }

        public ServiceResult AddBook(string title, string authorName, string? publisher, string genreName, int quantity, int available)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return ServiceResult.Failure("Title is required.");
            }

            var author = _unitOfWork.Authors.GetByName(authorName?.Trim() ?? string.Empty);
            var genre = _unitOfWork.Genres.GetByName(genreName?.Trim() ?? string.Empty);
            if (author == null || genre == null)
            {
                return ServiceResult.Failure("Invalid Author or Genre name!");
            }

            var book = new Book
            {
                Title = title.Trim(),
                AuthorId = author.AuthorId,
                Publisher = publisher?.Trim(),
                GenreId = genre.GenreId,
                Quantity = quantity,
                Available = available
            };

            _unitOfWork.Books.Add(book);
            _unitOfWork.SaveChanges();
            return ServiceResult.Success("Book added successfully!");
        }

        public ServiceResult UpdateBook(int bookId, string title, string authorName, string? publisher, string genreName, int quantity, int available)
        {
            var book = _unitOfWork.Books.GetById(bookId);
            if (book == null)
            {
                return ServiceResult.Failure("Book not found!");
            }

            var author = _unitOfWork.Authors.GetByName(authorName?.Trim() ?? string.Empty);
            var genre = _unitOfWork.Genres.GetByName(genreName?.Trim() ?? string.Empty);
            if (author == null || genre == null)
            {
                return ServiceResult.Failure("Invalid Author or Genre name!");
            }

            book.Title = title.Trim();
            book.AuthorId = author.AuthorId;
            book.Publisher = publisher?.Trim();
            book.GenreId = genre.GenreId;
            book.Quantity = quantity;
            book.Available = available;

            _unitOfWork.Books.Update(book);
            _unitOfWork.SaveChanges();
            return ServiceResult.Success("Book updated successfully!");
        }

        public ServiceResult DeleteBook(int bookId)
        {
            var book = _unitOfWork.Books.GetById(bookId);
            if (book == null)
            {
                return ServiceResult.Failure("Book not found!");
            }

            _unitOfWork.Books.Delete(book);
            _unitOfWork.SaveChanges();
            return ServiceResult.Success("Book deleted successfully!");
        }

        private static BookDto MapToDto(Book book) => new()
        {
            BookId = book.BookId,
            Title = book.Title,
            AuthorId = book.AuthorId,
            AuthorName = book.Author?.AuthorName ?? string.Empty,
            Publisher = book.Publisher,
            GenreId = book.GenreId,
            GenreName = book.Genre?.Name ?? string.Empty,
            Quantity = book.Quantity,
            Available = book.Available
        };
    }
}
