using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;

namespace A_PRN212.Application.Interfaces
{
    public interface IGenreService
    {
        IEnumerable<Genre> GetAllGenres();
        Genre? GetGenreById(int genreId);
        IEnumerable<Genre> SearchGenres(int? genreId, string? name, string? description);
        ServiceResult AddGenre(string name, string? description);
        ServiceResult UpdateGenre(int genreId, string name, string? description);
        ServiceResult DeleteGenre(int genreId);
    }
}
