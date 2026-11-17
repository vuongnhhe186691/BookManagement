using System.Windows.Input;
using A_PRN212.Application.Common;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Infrastructure.DependencyInjection;
using A_PRN212.MVVM;

namespace A_PRN212.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IDialogService _dialogService;
        private User _currentUser;

        private string _fullName = string.Empty;
        private string _gender = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string _role = string.Empty;
        private string _enteredPassword = string.Empty;

        public User CurrentUser => _currentUser;

        public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
        public string Gender { get => _gender; set => SetProperty(ref _gender, value); }
        public string Username { get => _username; set => SetProperty(ref _username, value); }
        public string Password { get => _password; set => SetProperty(ref _password, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Phone { get => _phone; set => SetProperty(ref _phone, value); }
        public string Role { get => _role; set => SetProperty(ref _role, value); }
        public string EnteredPassword { get => _enteredPassword; set => SetProperty(ref _enteredPassword, value); }

        public ICommand CheckPasswordCommand { get; }
        public ICommand SaveProfileCommand { get; }

        public event Action? PasswordVerified;
        public event Action? PasswordCheckFailed;

        public ProfileViewModel(User? user = null)
            : this(user ?? UserSession.Instance.CurrentUser ?? new User(),
                   ServiceProviderFactory.Instance.UserService,
                   ServiceProviderFactory.Instance.AuthService,
                   WpfDialogService.Instance)
        {
        }

        public ProfileViewModel(User user, IUserService userService, IAuthService authService, IDialogService dialogService)
        {
            _currentUser = user;
            _userService = userService;
            _authService = authService;
            _dialogService = dialogService;

            LoadFromUser(user);

            CheckPasswordCommand = new RelayCommand(ExecuteCheckPassword);
            SaveProfileCommand = new RelayCommand(ExecuteSaveProfile);
        }

        private void LoadFromUser(User user)
        {
            FullName = user.FullName;
            Gender = user.Gender ?? "Male";
            Username = user.Username;
            Password = user.Password;
            Email = user.Email;
            Phone = user.Phone ?? string.Empty;
            Role = user.Role;
        }

        public void ExecuteCheckPassword()
        {
            if (_authService.VerifyCurrentPassword(_currentUser, EnteredPassword))
            {
                _dialogService.ShowInfo("Password is correct");
                PasswordVerified?.Invoke();
            }
            else
            {
                _dialogService.ShowError("Password is incorrect");
                EnteredPassword = string.Empty;
                PasswordCheckFailed?.Invoke();
            }
        }

        public void ExecuteSaveProfile()
        {
            try
            {
                var result = _userService.UpdateProfile(_currentUser.UserId, FullName, Gender, Username, Password, Email, Phone);
                if (!result.IsSuccess || result.Data == null)
                {
                    _dialogService.ShowError(result.Message);
                    return;
                }

                _currentUser = result.Data;
                LoadFromUser(_currentUser);
                _dialogService.ShowInfo(result.Message);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }
    }
}
