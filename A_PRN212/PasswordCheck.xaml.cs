using System.Windows;
using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class PasswordCheck : Window
    {
        private readonly ProfileViewModel _viewModel;
        public bool IsPassword { get; private set; }

        public PasswordCheck()
            : this(UserSession.Instance.CurrentUser)
        {
        }

        public PasswordCheck(User? user)
        {
            InitializeComponent();
            _viewModel = new ProfileViewModel(user);
            _viewModel.PasswordVerified += () =>
            {
                IsPassword = true;
                NavigationHelper.NavigateTo(this, new ChangeInformation(_viewModel.CurrentUser));
            };
            _viewModel.PasswordCheckFailed += () =>
            {
                txtPassword.Clear();
                txtPassword.Focus();
            };
            DataContext = _viewModel;
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.EnteredPassword = txtPassword.Password;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new User_user(_viewModel.CurrentUser));

        private void Change_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new ForgotPassword());
    }
}
