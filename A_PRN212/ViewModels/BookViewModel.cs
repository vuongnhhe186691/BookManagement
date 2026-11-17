using System.Collections.ObjectModel;
using System.Windows.Input;
using A_PRN212.Application.Common;
using A_PRN212.Application.DTOs;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Infrastructure.DependencyInjection;
using A_PRN212.MVVM;

namespace A_PRN212.ViewModels
{
    public class BookViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly IBorrowService _borrowService;
        private readonly IDialogService _dialogService;
        private readonly User? _currentUser;

        private BookDto? _selectedBook;
        private string _bookIdText = string.Empty;
        private string _title = string.Empty;
        private string _authorName = string.Empty;
        private string _publisher = string.Empty;
        private string _genreName = string.Empty;
        private string _quantityText = string.Empty;
        private string _availableText = string.Empty;

        public ObservableCollection<BookDto> Books { get; } = new();

        public BookDto? SelectedBook
        {
            get => _selectedBook;
            set
            {
                if (SetProperty(ref _selectedBook, value) && value != null)
                {
                    BookIdText = value.BookId.ToString();
                    Title = value.Title;
                    AuthorName = value.AuthorName;
                    Publisher = value.Publisher ?? string.Empty;
                    GenreName = value.GenreName;
                    QuantityText = value.Quantity.ToString();
                    AvailableText = value.Available.ToString();
                }
            }
        }

        public string BookIdText { get => _bookIdText; set => SetProperty(ref _bookIdText, value); }
        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string AuthorName { get => _authorName; set => SetProperty(ref _authorName, value); }
        public string Publisher { get => _publisher; set => SetProperty(ref _publisher, value); }
        public string GenreName { get => _genreName; set => SetProperty(ref _genreName, value); }
        public string QuantityText { get => _quantityText; set => SetProperty(ref _quantityText, value); }
        public string AvailableText { get => _availableText; set => SetProperty(ref _availableText, value); }

        public ICommand LoadBooksCommand { get; }
        public ICommand InsertCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BorrowCommand { get; }

        public event Action<BorrowRecord>? BorrowSucceeded;

        public BookViewModel(User? currentUser = null)
            : this(currentUser ?? UserSession.Instance.CurrentUser,
                   ServiceProviderFactory.Instance.BookService,
                   ServiceProviderFactory.Instance.BorrowService,
                   WpfDialogService.Instance)
        {
        }

        public BookViewModel(User? currentUser, IBookService bookService, IBorrowService borrowService, IDialogService dialogService)
        {
            _currentUser = currentUser;
            _bookService = bookService;
            _borrowService = borrowService;
            _dialogService = dialogService;

            LoadBooksCommand = new RelayCommand(LoadBooks);
            InsertCommand = new RelayCommand(ExecuteInsert);
            UpdateCommand = new RelayCommand(ExecuteUpdate);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            SearchCommand = new RelayCommand(ExecuteSearch);
            ClearCommand = new RelayCommand(ExecuteClear);
            BorrowCommand = new RelayCommand(ExecuteBorrow);

            LoadBooks();
        }

        public void LoadBooks()
        {
            try
            {
                ReplaceItems(_bookService.GetAllBooks());
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error loading books: {ex.Message}");
            }
        }

        private void ExecuteInsert()
        {
            try
            {
                if (!int.TryParse(QuantityText, out int quantity) || !int.TryParse(AvailableText, out int available))
                {
                    _dialogService.ShowWarning("Invalid input! Please enter numbers for Quantity and Available.");
                    return;
                }

                var result = _bookService.AddBook(Title, AuthorName, Publisher, GenreName, quantity, available);
                if (!result.IsSuccess)
                {
                    _dialogService.ShowWarning(result.Message);
                    return;
                }

                _dialogService.ShowInfo(result.Message);
                LoadBooks();
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
                if (!int.TryParse(BookIdText, out int bookId))
                {
                    _dialogService.ShowWarning("Invalid ID! Please enter a valid Book ID.");
                    return;
                }

                if (!int.TryParse(QuantityText, out int quantity) || !int.TryParse(AvailableText, out int available))
                {
                    _dialogService.ShowWarning("Invalid input! Please enter numbers for Quantity and Available.");
                    return;
                }

                var result = _bookService.UpdateBook(bookId, Title, AuthorName, Publisher, GenreName, quantity, available);
                if (!result.IsSuccess)
                {
                    _dialogService.ShowWarning(result.Message);
                    return;
                }

                LoadBooks();
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
                if (!int.TryParse(BookIdText, out int bookId))
                {
                    _dialogService.ShowWarning("Invalid ID! Please enter a valid Book ID.");
                    return;
                }

                var result = _bookService.DeleteBook(bookId);
                if (!result.IsSuccess)
                {
                    _dialogService.ShowWarning(result.Message);
                    return;
                }

                LoadBooks();
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
                var results = _bookService.SearchBooks(Title, Publisher).ToList();
                if (results.Count == 0)
                {
                    _dialogService.ShowInfo("No books found!", "Search Result");
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
            SelectedBook = null;
            BookIdText = string.Empty;
            Title = string.Empty;
            AuthorName = string.Empty;
            Publisher = string.Empty;
            GenreName = string.Empty;
            QuantityText = string.Empty;
            AvailableText = string.Empty;
            LoadBooks();
        }

        private void ExecuteBorrow()
        {
            try
            {
                if (SelectedBook == null)
                {
                    _dialogService.ShowError("Please select a book to borrow.");
                    return;
                }

                var user = _currentUser ?? UserSession.Instance.CurrentUser;
                if (user == null)
                {
                    _dialogService.ShowError("User session not found.");
                    return;
                }

                var result = _borrowService.BorrowBook(user.UserId, SelectedBook.BookId);
                if (!result.IsSuccess || result.Data == null)
                {
                    _dialogService.ShowError(result.Message);
                    return;
                }

                _dialogService.ShowInfo(result.Message, "Success");
                BorrowSucceeded?.Invoke(result.Data);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error: {ex.Message}\nInner Exception: {ex.InnerException?.Message}");
            }
        }

        private void ReplaceItems(IEnumerable<BookDto> items)
        {
            Books.Clear();
            foreach (var item in items)
            {
                Books.Add(item);
            }
        }
    }
}
