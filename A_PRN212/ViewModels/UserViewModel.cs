using System.Collections.ObjectModel;
using System.Windows.Input;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Infrastructure.DependencyInjection;
using A_PRN212.MVVM;

namespace A_PRN212.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IDialogService _dialogService;
        private readonly bool _isAdminMode;

        private User? _selectedUser;
        private string _userIdText = string.Empty;
        private string _fullName = string.Empty;
        private string _gender = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string _role = string.Empty;

        public ObservableCollection<User> Users { get; } = new();

        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value) && value != null)
                {
                    UserIdText = value.UserId.ToString();
                    FullName = value.FullName;
                    Gender = value.Gender ?? string.Empty;
                    Username = value.Username;
                    Password = value.Password;
                    Email = value.Email;
                    Phone = value.Phone ?? string.Empty;
                    Role = value.Role;
                }
            }
        }

        public string UserIdText { get => _userIdText; set => SetProperty(ref _userIdText, value); }
        public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
        public string Gender { get => _gender; set => SetProperty(ref _gender, value); }
        public string Username { get => _username; set => SetProperty(ref _username, value); }
        public string Password { get => _password; set => SetProperty(ref _password, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Phone { get => _phone; set => SetProperty(ref _phone, value); }
        public string Role { get => _role; set => SetProperty(ref _role, value); }

        public ICommand LoadUsersCommand { get; }
        public ICommand InsertCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }

        public UserViewModel(bool isAdminMode = true)
            : this(isAdminMode, ServiceProviderFactory.Instance.UserService, WpfDialogService.Instance)
        {
        }

        public UserViewModel(bool isAdminMode, IUserService userService, IDialogService dialogService)
        {
            _isAdminMode = isAdminMode;
            _userService = userService;
            _dialogService = dialogService;

            LoadUsersCommand = new RelayCommand(LoadUsers);
            InsertCommand = new RelayCommand(ExecuteInsert);
            UpdateCommand = new RelayCommand(ExecuteUpdate);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            SearchCommand = new RelayCommand(ExecuteSearch);
            ClearCommand = new RelayCommand(ExecuteClear);

            LoadUsers();
        }

        public void LoadUsers()
        {
            try
            {
                ReplaceItems(_userService.GetAllUsers());
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error loading users: {ex.Message}");
            }
        }

        private void ExecuteInsert()
        {
            try
            {
                string? password = _isAdminMode ? Password : null;
                var result = _userService.AddUser(FullName, Gender, Username, password, Email, Phone, Role);
                if (!result.IsSuccess)
                {
                    _dialogService.ShowWarning(result.Message);
                    return;
                }

                _dialogService.ShowInfo(result.Message);
                LoadUsers();
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
                if (!int.TryParse(UserIdText, out int userId))
                {
                    _dialogService.ShowWarning("Invalid input! Please enter numbers for UserId.");
                    return;
                }

                string? password = _isAdminMode ? Password : null;
                var result = _userService.UpdateUser(userId, FullName, Gender, Username, password, Email, Phone, Role);
                if (!result.IsSuccess)
                {
                    _dialogService.ShowWarning(result.Message);
                    return;
                }

                LoadUsers();
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
                if (!int.TryParse(UserIdText, out int userId))
                {
                    _dialogService.ShowWarning("Invalid input! Please enter numbers for UserId.");
                    return;
                }

                var result = _userService.DeleteUser(userId);
                if (!result.IsSuccess)
                {
                    _dialogService.ShowWarning(result.Message);
                    return;
                }

                LoadUsers();
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
                int? userId = int.TryParse(UserIdText, out int parsedId) ? parsedId : null;
                var results = _userService.SearchUsers(userId, FullName, Username, Email, Role).ToList();

                if (results.Count == 0)
                {
                    _dialogService.ShowInfo("No users found!", "Search Result");
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
            SelectedUser = null;
            UserIdText = string.Empty;
            FullName = string.Empty;
            Gender = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Role = string.Empty;
            LoadUsers();
        }

        private void ReplaceItems(IEnumerable<User> items)
        {
            Users.Clear();
            foreach (var item in items)
            {
                Users.Add(item);
            }
        }
    }
}
