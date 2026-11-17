using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;

namespace A_PRN212.Application.Interfaces
{
    public interface IAuthorService
    {
        IEnumerable<Author> GetAllAuthors();
        IEnumerable<Author> SearchAuthors(int? authorId, string? name, string? biography, string? nationality, DateTime? birthDate);
        ServiceResult AddAuthor(string name, string? biography, string nationality, DateTime? birthDate);
        ServiceResult UpdateAuthor(int authorId, string name, string? biography, string nationality, DateTime? birthDate);
        ServiceResult DeleteAuthor(int authorId);
    }
}
