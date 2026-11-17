using System.Windows.Input;
using A_PRN212.Application.Common;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Infrastructure.DependencyInjection;
using A_PRN212.MVVM;

namespace A_PRN212.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;

        public User? CurrentUser => UserSession.Instance.CurrentUser;
        public string WelcomeTitle => CurrentUser != null
            ? $"Welcome, {CurrentUser.FullName} ({CurrentUser.Role})"
            : "Library Management System";

        public ICommand LogoutCommand { get; }

        public event Action? LoggedOut;

        public HomeViewModel()
            : this(ServiceProviderFactory.Instance.AuthService)
        {
        }

        public HomeViewModel(IAuthService authService)
        {
            _authService = authService;
            LogoutCommand = new RelayCommand(ExecuteLogout);
        }

        public void ExecuteLogout()
        {
            _authService.Logout();
            LoggedOut?.Invoke();
        }
    }
}
