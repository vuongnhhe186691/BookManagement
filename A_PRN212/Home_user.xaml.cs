using System.Windows;
using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class Home_user : Window
    {
        private readonly HomeViewModel _viewModel;
        private readonly User? _currentUser;

        public Home_user()
            : this(UserSession.Instance.CurrentUser)
        {
        }

        public Home_user(User? user)
        {
            InitializeComponent();
            _currentUser = user ?? UserSession.Instance.CurrentUser;
            if (_currentUser != null)
            {
                UserSession.Instance.SetUser(_currentUser);
            }

            _viewModel = new HomeViewModel();
            _viewModel.LoggedOut += () => NavigationHelper.NavigateTo(this, new Login());
            DataContext = _viewModel;
        }

        private void btnAuthor_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new Author_user());

        private void btnGenre_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new Genre_user());

        private void btnBook_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new Book_user(_currentUser));

        private void btnLogout_Click(object sender, RoutedEventArgs e)
            => _viewModel.ExecuteLogout();

        private void btnUser_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new User_user(_currentUser));

        private void btnBorrowRecord_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new Borrow_userlist(_currentUser));

        private void btnRule_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new Rule_user(_currentUser));
    }
}
