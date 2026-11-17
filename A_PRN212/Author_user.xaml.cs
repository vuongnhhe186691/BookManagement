using System.Windows;
using A_PRN212.Domain.Enums;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class Author_user : Window
    {
        public Author_user()
        {
            InitializeComponent();
            DataContext = new AuthorViewModel();
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateHome(this, UserRole.Reader);
    }
}
