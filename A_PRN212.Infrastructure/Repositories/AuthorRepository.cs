using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Interfaces;
using A_PRN212.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace A_PRN212.Infrastructure.Repositories
{
    public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(LibraryDbContext context) : base(context)
        {
        }

        public Author? GetByName(string authorName)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(a => a.AuthorName == authorName);
        }

        public IEnumerable<Author> SearchAuthors(int? authorId, string? name, string? biography, string? nationality, DateTime? birthDate)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (authorId.HasValue)
            {
                query = query.Where(a => a.AuthorId == authorId.Value);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                string trimmedName = name.Trim();
                query = query.Where(a => a.AuthorName.Contains(trimmedName));
            }

            if (!string.IsNullOrWhiteSpace(biography))
            {
                string trimmedBio = biography.Trim();
                query = query.Where(a => a.Biography != null && a.Biography.Contains(trimmedBio));
            }

            if (!string.IsNullOrWhiteSpace(nationality))
            {
                string trimmedNat = nationality.Trim();
                query = query.Where(a => a.Nationality != null && a.Nationality.Contains(trimmedNat));
            }

            if (birthDate.HasValue)
            {
                DateTime date = birthDate.Value.Date;
                query = query.Where(a => a.BirthDate.HasValue && a.BirthDate.Value.Date == date);
            }

            return query.ToList();
        }
    }
}
