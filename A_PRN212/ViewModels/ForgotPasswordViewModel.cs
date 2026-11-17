using System.Windows.Input;
using A_PRN212.Application.Interfaces;
using A_PRN212.Infrastructure.DependencyInjection;
using A_PRN212.MVVM;

namespace A_PRN212.ViewModels
{
    public class ForgotPasswordViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IDialogService _dialogService;

        private string _email = string.Empty;
        private string _otpCode = string.Empty;

        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string OtpCode { get => _otpCode; set => SetProperty(ref _otpCode, value); }

        public ICommand SendOtpCommand { get; }
        public ICommand VerifyOtpCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<int>? OtpVerified;
        public event Action? Cancelled;

        public ForgotPasswordViewModel()
            : this(ServiceProviderFactory.Instance.AuthService, WpfDialogService.Instance)
        {
        }

        public ForgotPasswordViewModel(IAuthService authService, IDialogService dialogService)
        {
            _authService = authService;
            _dialogService = dialogService;

            SendOtpCommand = new RelayCommand(ExecuteSendOtp);
            VerifyOtpCommand = new RelayCommand(ExecuteVerifyOtp);
            CancelCommand = new RelayCommand(() => Cancelled?.Invoke());
        }

        private void ExecuteSendOtp()
        {
            var result = _authService.SendPasswordResetOtp(Email);
            if (!result.IsSuccess)
            {
                _dialogService.ShowError(result.Message, "Error");
                return;
            }

            _dialogService.ShowInfo(result.Message, "Success");
        }

        private void ExecuteVerifyOtp()
        {
            var result = _authService.VerifyOtp(Email, OtpCode);
            if (!result.IsSuccess)
            {
                _dialogService.ShowError(result.Message, "Error");
                return;
            }

            _dialogService.ShowInfo(result.Message, "Success");
            OtpVerified?.Invoke(result.Data);
        }
    }
}
