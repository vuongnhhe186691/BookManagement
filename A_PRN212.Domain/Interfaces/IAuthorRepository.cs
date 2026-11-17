using A_PRN212.Domain.Entities;

namespace A_PRN212.Domain.Interfaces
{
    public interface IAuthorRepository : IGenericRepository<Author>
    {
        Author? GetByName(string authorName);
        IEnumerable<Author> SearchAuthors(int? authorId, string? name, string? biography, string? nationality, DateTime? birthDate);
    }
}
