using System.Windows;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class Home_lib : Window
    {
        private readonly HomeViewModel _viewModel;

        public Home_lib()
        {
            InitializeComponent();
            _viewModel = new HomeViewModel();
            _viewModel.LoggedOut += () => NavigationHelper.NavigateTo(this, new Login());
            DataContext = _viewModel;
        }

        public void btnBook_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new Book_lib());

        public void btnAuthor_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new Author_lib());

        public void btnGenre_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new Genre_lib());

        public void btnUser_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new User_lib());

        public void btnBorrowRecord_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateTo(this, new BorrowRecord_lib());

        public void btnLogout_Click(object sender, RoutedEventArgs e)
            => _viewModel.ExecuteLogout();
    }
}
