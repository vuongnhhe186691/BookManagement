using System.Windows;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class ForgotPassword : Window
    {
        private readonly ForgotPasswordViewModel _viewModel;

        public ForgotPassword()
        {
            InitializeComponent();
            _viewModel = new ForgotPasswordViewModel();
            _viewModel.OtpVerified += userId => NavigationHelper.NavigateTo(this, new ChangePassword(userId));
            _viewModel.Cancelled += () => NavigationHelper.NavigateTo(this, new Login());
            DataContext = _viewModel;
        }
    }
}
