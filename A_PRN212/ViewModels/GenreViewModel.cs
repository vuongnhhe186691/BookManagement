using System.Collections.ObjectModel;
using System.Windows.Input;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Infrastructure.DependencyInjection;
using A_PRN212.MVVM;

namespace A_PRN212.ViewModels
{
    public class GenreViewModel : ViewModelBase
    {
        private readonly IGenreService _genreService;
        private readonly IDialogService _dialogService;

        private Genre? _selectedGenre;
        private string _genreIdText = string.Empty;
        private string _name = string.Empty;
        private string _description = string.Empty;

        public ObservableCollection<Genre> Genres { get; } = new();

        public Genre? SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                if (SetProperty(ref _selectedGenre, value) && value != null)
                {
                    GenreIdText = value.GenreId.ToString();
                    Name = value.Name;
                    Description = value.Description ?? string.Empty;
                }
            }
        }

        public string GenreIdText { get => _genreIdText; set => SetProperty(ref _genreIdText, value); }
        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Description { get => _description; set => SetProperty(ref _description, value); }

        public ICommand LoadGenresCommand { get; }
        public ICommand InsertCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }

        public GenreViewModel()
            : this(ServiceProviderFactory.Instance.GenreService, WpfDialogService.Instance)
        {
        }

        public GenreViewModel(IGenreService genreService, IDialogService dialogService)
        {
            _genreService = genreService;
            _dialogService = dialogService;

            LoadGenresCommand = new RelayCommand(LoadGenres);
            InsertCommand = new RelayCommand(ExecuteInsert);
            UpdateCommand = new RelayCommand(ExecuteUpdate);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            SearchCommand = new RelayCommand(ExecuteSearch);
            ClearCommand = new RelayCommand(ExecuteClear);

            LoadGenres();
        }

        public void LoadGenres()
        {
            try
            {
                ReplaceItems(_genreService.GetAllGenres());
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error loading genres: {ex.Message}");
            }
        }

        private void ExecuteInsert()
        {
            try
            {
                var result = _genreService.AddGenre(Name, Description);
                if (!result.IsSuccess)
                {
                    _dialogService.ShowWarning(result.Message);
                    return;
                }

                _dialogService.ShowInfo(result.Message);
                LoadGenres();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }

        private void ExecuteUpdate()
        {
            try
            {
                if (!int.TryParse(GenreIdText, out int genreId))
                {
                    _dialogService.ShowWarning("Invalid input! Please enter numbers for GenreId.");
                    return;
                }

                var result = _genreService.UpdateGenre(genreId, Name, Description);
                if (!result.IsSuccess)
                {
                    _dialogService.ShowWarning(result.Message);
                    return;
                }

                _dialogService.ShowInfo(result.Message);
                LoadGenres();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }

        private void ExecuteDelete()
        {
            try
            {
                if (!int.TryParse(GenreIdText, out int genreId))
                {
                    _dialogService.ShowWarning("Invalid input! Please enter numbers for GenreId.");
                    return;
                }

                var result = _genreService.DeleteGenre(genreId);
                if (!result.IsSuccess)
                {
                    _dialogService.ShowWarning(result.Message);
                    return;
                }

                _dialogService.ShowInfo(result.Message);
                LoadGenres();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }

        private void ExecuteSearch()
        {
            try
            {
                int? genreId = int.TryParse(GenreIdText, out int parsedId) ? parsedId : null;
                var results = _genreService.SearchGenres(genreId, Name, Description).ToList();

                if (results.Count == 0)
                {
                    _dialogService.ShowInfo("Genre not found!", "Search Result");
                }
                else if (results.Count == 1 && genreId.HasValue)
                {
                    Name = results[0].Name;
                    Description = results[0].Description ?? string.Empty;
                }

                ReplaceItems(results);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }

        private void ExecuteClear()
        {
            SelectedGenre = null;
            GenreIdText = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
            LoadGenres();
        }

        private void ReplaceItems(IEnumerable<Genre> items)
        {
            Genres.Clear();
            foreach (var item in items)
            {
                Genres.Add(item);
            }
        }
    }
}
