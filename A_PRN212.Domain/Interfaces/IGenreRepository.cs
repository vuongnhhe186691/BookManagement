using A_PRN212.Domain.Entities;

namespace A_PRN212.Domain.Interfaces
{
    public interface IGenreRepository : IGenericRepository<Genre>
    {
        Genre? GetByName(string name);
        IEnumerable<Genre> SearchGenres(int? genreId, string? name, string? description);
    }
}
