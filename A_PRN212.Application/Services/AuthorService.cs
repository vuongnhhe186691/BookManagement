using A_PRN212.Application.Common;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Interfaces;

namespace A_PRN212.Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Author> GetAllAuthors()
        {
            return _unitOfWork.Authors.GetAll().ToList();
        }

        public IEnumerable<Author> SearchAuthors(int? authorId, string? name, string? biography, string? nationality, DateTime? birthDate)
        {
            return _unitOfWork.Authors.SearchAuthors(authorId, name, biography, nationality, birthDate).ToList();
        }

        public ServiceResult AddAuthor(string name, string? biography, string nationality, DateTime? birthDate)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(nationality) || !birthDate.HasValue)
            {
                return ServiceResult.Failure("Please fill all required fields!");
            }

            var newAuthor = new Author
            {
                AuthorName = name.Trim(),
                Biography = biography?.Trim(),
                Nationality = nationality.Trim(),
                BirthDate = birthDate.Value
            };

            _unitOfWork.Authors.Add(newAuthor);
            _unitOfWork.SaveChanges();
            return ServiceResult.Success("Author added successfully!");
        }

        public ServiceResult UpdateAuthor(int authorId, string name, string? biography, string nationality, DateTime? birthDate)
        {
            var existingAuthor = _unitOfWork.Authors.GetById(authorId);
            if (existingAuthor == null)
            {
                return ServiceResult.Failure("Author not found!");
            }

            existingAuthor.AuthorName = name.Trim();
            existingAuthor.Biography = biography?.Trim();
            existingAuthor.Nationality = nationality.Trim();
            if (birthDate.HasValue)
            {
                existingAuthor.BirthDate = birthDate.Value;
            }

            _unitOfWork.Authors.Update(existingAuthor);
            _unitOfWork.SaveChanges();
            return ServiceResult.Success("Author updated successfully!");
        }

        public ServiceResult DeleteAuthor(int authorId)
        {
            var author = _unitOfWork.Authors.GetById(authorId);
            if (author == null)
            {
                return ServiceResult.Failure("Author not found!");
            }

            _unitOfWork.Authors.Delete(author);
            _unitOfWork.SaveChanges();
            return ServiceResult.Success("Author deleted successfully!");
        }
    }
}
