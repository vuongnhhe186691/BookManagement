using System.Windows.Input;
using A_PRN212.Application.Interfaces;
using A_PRN212.Infrastructure.DependencyInjection;
using A_PRN212.MVVM;

namespace A_PRN212.ViewModels
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IDialogService _dialogService;
        private readonly int _userId;

        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;

        public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }
        public string ConfirmPassword { get => _confirmPassword; set => SetProperty(ref _confirmPassword, value); }

        public ICommand ChangePasswordCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action? PasswordChanged;
        public event Action? Cancelled;

        public ChangePasswordViewModel(int userId)
            : this(userId, ServiceProviderFactory.Instance.AuthService, WpfDialogService.Instance)
        {
        }

        public ChangePasswordViewModel(int userId, IAuthService authService, IDialogService dialogService)
        {
            _userId = userId;
            _authService = authService;
            _dialogService = dialogService;

            ChangePasswordCommand = new RelayCommand(ExecuteChangePassword);
            CancelCommand = new RelayCommand(() => Cancelled?.Invoke());
        }

        public void ExecuteChangePassword()
        {
            var result = _authService.ChangePassword(_userId, NewPassword, ConfirmPassword);
            if (!result.IsSuccess)
            {
                _dialogService.ShowWarning(result.Message, "Warning");
                return;
            }

            _dialogService.ShowInfo(result.Message, "Success");
            PasswordChanged?.Invoke();
        }
    }
}
