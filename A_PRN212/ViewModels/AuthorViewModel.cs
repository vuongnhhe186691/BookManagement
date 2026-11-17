using System.Collections.ObjectModel;
using System.Windows.Input;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Infrastructure.DependencyInjection;
using A_PRN212.MVVM;

namespace A_PRN212.ViewModels
{
    public class AuthorViewModel : ViewModelBase
    {
        private readonly IAuthorService _authorService;
        private readonly IDialogService _dialogService;

        private Author? _selectedAuthor;
        private string _authorIdText = string.Empty;
        private string _authorName = string.Empty;
        private string _biography = string.Empty;
        private string _nationality = string.Empty;
        private DateTime? _birthDate;

        public ObservableCollection<Author> Authors { get; } = new();

        public Author? SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                if (SetProperty(ref _selectedAuthor, value) && value != null)
                {
                    AuthorIdText = value.AuthorId.ToString();
                    AuthorName = value.AuthorName;
                    Biography = value.Biography ?? string.Empty;
                    Nationality = value.Nationality ?? string.Empty;
                    BirthDate = value.BirthDate;
                }
            }
        }

        public string AuthorIdText { get => _authorIdText; set => SetProperty(ref _authorIdText, value); }
        public string AuthorName { get => _authorName; set => SetProperty(ref _authorName, value); }
        public string Biography { get => _biography; set => SetProperty(ref _biography, value); }
        public string Nationality { get => _nationality; set => SetProperty(ref _nationality, value); }
        public DateTime? BirthDate { get => _birthDate; set => SetProperty(ref _birthDate, value); }

        public ICommand LoadAuthorsCommand { get; }
        public ICommand InsertCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }

        public AuthorViewModel()
            : this(ServiceProviderFactory.Instance.AuthorService, WpfDialogService.Instance)
        {
        }

        public AuthorViewModel(IAuthorService authorService, IDialogService dialogService)
        {
            _authorService = authorService;
            _dialogService = dialogService;

            LoadAuthorsCommand = new RelayCommand(LoadAuthors);
            InsertCommand = new RelayCommand(ExecuteInsert);
            UpdateCommand = new RelayCommand(ExecuteUpdate);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            SearchCommand = new RelayCommand(ExecuteSearch);
            ClearCommand = new RelayCommand(ExecuteClear);

            LoadAuthors();
        }

        public void LoadAuthors()
        {
            try
            {
                ReplaceItems(_authorService.GetAllAuthors());
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error loading authors: {ex.Message}");
            }
        }

        private void ExecuteInsert()
        {
            try
            {
                var result = _authorService.AddAuthor(AuthorName, Biography, Nationality, BirthDate);
                if (!result.IsSuccess)
                {
                    _dialogService.ShowWarning(result.Message);
                    return;
                }

                LoadAuthors();
                _dialogService.ShowInfo(result.Message);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error: {ex.Message}");
            }
        }

        private void ExecuteUpdate()
        {
            try
            {
                if (!int.TryParse(AuthorIdText, out int authorId))
                {
                    _dialogService.ShowWarning("Invalid ID!");
                    return;
                }

                var result = _authorService.UpdateAuthor(authorId, AuthorName, Biography, Nationality, BirthDate);
                if (!result.IsSuccess)
                {
                    _dialogService.ShowWarning(result.Message);
                    return;
                }

                LoadAuthors();
                _dialogService.ShowInfo(result.Message);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error: {ex.Message}");
            }
        }

        private void ExecuteDelete()
        {
            try
            {
                if (!int.TryParse(AuthorIdText, out int authorId))
                {
                    _dialogService.ShowWarning("Invalid ID!");
                    return;
                }

                var result = _authorService.DeleteAuthor(authorId);
                if (!result.IsSuccess)
                {
                    _dialogService.ShowWarning(result.Message);
                    return;
                }

                LoadAuthors();
                _dialogService.ShowInfo(result.Message);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error: {ex.Message}");
            }
        }

        private void ExecuteSearch()
        {
            try
            {
                int? authorId = int.TryParse(AuthorIdText, out int parsedId) ? parsedId : null;
                var results = _authorService.SearchAuthors(authorId, AuthorName, Biography, Nationality, BirthDate).ToList();

                if (results.Count == 0)
                {
                    _dialogService.ShowInfo("No authors found!", "Search Result");
                }

                ReplaceItems(results);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error: {ex.Message}");
            }
        }

        private void ExecuteClear()
        {
            SelectedAuthor = null;
            AuthorIdText = string.Empty;
            AuthorName = string.Empty;
            Biography = string.Empty;
            Nationality = string.Empty;
            BirthDate = null;
            LoadAuthors();
        }

        private void ReplaceItems(IEnumerable<Author> items)
        {
            Authors.Clear();
            foreach (var item in items)
            {
                Authors.Add(item);
            }
        }
    }
}
