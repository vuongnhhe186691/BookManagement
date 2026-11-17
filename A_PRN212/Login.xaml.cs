using System.Windows;
using A_PRN212.Domain.Entities;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class Login : Window
    {
        private readonly LoginViewModel _viewModel;

        public Login()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            _viewModel.LoginSucceeded += OnLoginSucceeded;
            _viewModel.NavigateToSignUp += () => NavigationHelper.NavigateTo(this, new SignUp());
            _viewModel.NavigateToForgotPassword += () => NavigationHelper.NavigateTo(this, new ForgotPassword());
            DataContext = _viewModel;
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = txtPassword.Password;
        }

        private void OnLoginSucceeded(User user)
        {
            var homeWindow = NavigationHelper.CreateHomeWindow(user);
            NavigationHelper.NavigateTo(this, homeWindow);
        }
    }
}
