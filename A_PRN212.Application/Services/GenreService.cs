using A_PRN212.Application.Common;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Interfaces;

namespace A_PRN212.Application.Services
{
    public class GenreService : IGenreService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Genre> GetAllGenres()
        {
            return _unitOfWork.Genres.GetAll().ToList();
        }

        public Genre? GetGenreById(int genreId)
        {
            return _unitOfWork.Genres.GetById(genreId);
        }

        public IEnumerable<Genre> SearchGenres(int? genreId, string? name, string? description)
        {
            return _unitOfWork.Genres.SearchGenres(genreId, name, description).ToList();
        }

        public ServiceResult AddGenre(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return ServiceResult.Failure("Genre name is required!");
            }

            string trimmedName = name.Trim();
            if (_unitOfWork.Genres.GetByName(trimmedName) != null)
            {
                return ServiceResult.Failure("Genre name already exists!");
            }

            var genre = new Genre
            {
                Name = trimmedName,
                Description = description?.Trim()
            };

            _unitOfWork.Genres.Add(genre);
            _unitOfWork.SaveChanges();
            return ServiceResult.Success("Insert successfully!");
        }

        public ServiceResult UpdateGenre(int genreId, string name, string? description)
        {
            var genre = _unitOfWork.Genres.GetById(genreId);
            if (genre == null)
            {
                return ServiceResult.Failure("Genre not found!");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return ServiceResult.Failure("Genre name is required!");
            }

            genre.Name = name.Trim();
            genre.Description = description?.Trim();

            _unitOfWork.Genres.Update(genre);
            _unitOfWork.SaveChanges();
            return ServiceResult.Success("Update successfully!");
        }

        public ServiceResult DeleteGenre(int genreId)
        {
            var genre = _unitOfWork.Genres.GetById(genreId);
            if (genre == null)
            {
                return ServiceResult.Failure("Genre not found!");
            }

            _unitOfWork.Genres.Delete(genre);
            _unitOfWork.SaveChanges();
            return ServiceResult.Success("Delete successfully!");
        }
    }
}
