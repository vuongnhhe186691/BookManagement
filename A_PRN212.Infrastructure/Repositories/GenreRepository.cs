using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Interfaces;
using A_PRN212.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace A_PRN212.Infrastructure.Repositories
{
    public class GenreRepository : GenericRepository<Genre>, IGenreRepository
    {
        public GenreRepository(LibraryDbContext context) : base(context)
        {
        }

        public Genre? GetByName(string name)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(g => g.Name == name);
        }

        public IEnumerable<Genre> SearchGenres(int? genreId, string? name, string? description)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (genreId.HasValue)
            {
                query = query.Where(g => g.GenreId == genreId.Value);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                string trimmedName = name.Trim();
                query = query.Where(g => g.Name.Contains(trimmedName));
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                string trimmedDesc = description.Trim();
                query = query.Where(g => g.Description != null && g.Description.Contains(trimmedDesc));
            }

            return query.ToList();
        }
    }
}
