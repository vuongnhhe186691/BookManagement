using System.Windows.Input;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Infrastructure.DependencyInjection;
using A_PRN212.MVVM;

namespace A_PRN212.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IDialogService _dialogService;

        private string _username = string.Empty;
        private string _password = string.Empty;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateSignUpCommand { get; }
        public ICommand NavigateForgotPasswordCommand { get; }

        public event Action<User>? LoginSucceeded;
        public event Action? NavigateToSignUp;
        public event Action? NavigateToForgotPassword;

        public LoginViewModel()
            : this(ServiceProviderFactory.Instance.AuthService, WpfDialogService.Instance)
        {
        }

        public LoginViewModel(IAuthService authService, IDialogService dialogService)
        {
            _authService = authService;
            _dialogService = dialogService;

            LoginCommand = new RelayCommand(ExecuteLogin);
            NavigateSignUpCommand = new RelayCommand(() => NavigateToSignUp?.Invoke());
            NavigateForgotPasswordCommand = new RelayCommand(() => NavigateToForgotPassword?.Invoke());
        }

        public void ExecuteLogin()
        {
            var result = _authService.Login(Username, Password);
            if (!result.IsSuccess || result.Data == null)
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    _dialogService.ShowWarning(result.Message, "Error");
                }
                else
                {
                    _dialogService.ShowError(result.Message, "Login Failed");
                }
                return;
            }

            LoginSucceeded?.Invoke(result.Data);
        }
    }
}
