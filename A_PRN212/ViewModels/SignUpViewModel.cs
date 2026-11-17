using System.Windows.Input;
using A_PRN212.Application.Interfaces;
using A_PRN212.Infrastructure.DependencyInjection;
using A_PRN212.MVVM;

namespace A_PRN212.ViewModels
{
    public class SignUpViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IDialogService _dialogService;

        private string _fullName = string.Empty;
        private string _gender = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;

        public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
        public string Gender { get => _gender; set => SetProperty(ref _gender, value); }
        public string Username { get => _username; set => SetProperty(ref _username, value); }
        public string Password { get => _password; set => SetProperty(ref _password, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Phone { get => _phone; set => SetProperty(ref _phone, value); }

        public ICommand SignUpCommand { get; }
        public ICommand NavigateLoginCommand { get; }

        public event Action? SignUpSucceeded;
        public event Action? NavigateToLogin;

        public SignUpViewModel()
            : this(ServiceProviderFactory.Instance.AuthService, WpfDialogService.Instance)
        {
        }

        public SignUpViewModel(IAuthService authService, IDialogService dialogService)
        {
            _authService = authService;
            _dialogService = dialogService;

            SignUpCommand = new RelayCommand(ExecuteSignUp);
            NavigateLoginCommand = new RelayCommand(() => NavigateToLogin?.Invoke());
        }

        public void ExecuteSignUp()
        {
            var result = _authService.Register(FullName, Gender, Username, Password, Email, Phone);
            if (!result.IsSuccess)
            {
                _dialogService.ShowWarning(result.Message, "Sign Up");
                return;
            }

            _dialogService.ShowInfo(result.Message, "Success");
            SignUpSucceeded?.Invoke();
        }
    }
}
