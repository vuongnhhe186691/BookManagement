using System.Windows;
using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class ChangeInformation : Window
    {
        private readonly ProfileViewModel _viewModel;

        public ChangeInformation()
            : this(UserSession.Instance.CurrentUser)
        {
        }

        public ChangeInformation(User? user)
        {
            InitializeComponent();
            _viewModel = new ProfileViewModel(user);
            DataContext = _viewModel;
            txtPassword.Password = _viewModel.Password;
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = txtPassword.Password;
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new User_user(_viewModel.CurrentUser));
    }
}
