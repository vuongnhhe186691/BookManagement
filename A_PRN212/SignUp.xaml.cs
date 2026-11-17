using System.Windows;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class SignUp : Window
    {
        private readonly SignUpViewModel _viewModel;

        public SignUp()
        {
            InitializeComponent();
            _viewModel = new SignUpViewModel();
            _viewModel.SignUpSucceeded += () => NavigationHelper.NavigateTo(this, new Login());
            _viewModel.NavigateToLogin += () => NavigationHelper.NavigateTo(this, new Login());
            DataContext = _viewModel;
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = txtPassword.Password;
        }
    }
}
