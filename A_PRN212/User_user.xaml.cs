using System.Windows;
using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Enums;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class User_user : Window
    {
        private readonly ProfileViewModel _viewModel;

        public User_user()
            : this(UserSession.Instance.CurrentUser)
        {
        }

        public User_user(User? user)
        {
            InitializeComponent();
            _viewModel = new ProfileViewModel(user);
            DataContext = _viewModel;
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new PasswordCheck(_viewModel.CurrentUser));

        private void btnReturn_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateHome(this, UserRole.Reader);
    }
}
