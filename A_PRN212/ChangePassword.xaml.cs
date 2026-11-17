using System.Windows;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class ChangePassword : Window
    {
        private readonly ChangePasswordViewModel _viewModel;

        public ChangePassword(int userId)
        {
            InitializeComponent();
            _viewModel = new ChangePasswordViewModel(userId);
            _viewModel.PasswordChanged += () => NavigationHelper.NavigateTo(this, new Login());
            _viewModel.Cancelled += () => NavigationHelper.NavigateTo(this, new ForgotPassword());
            DataContext = _viewModel;
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.NewPassword = txtPassword.Password;
        }

        private void txtConfirm_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.ConfirmPassword = txtConfirm.Password;
        }
    }
}
