using System.Windows;
using A_PRN212.Domain.Enums;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class Book_lib : Window
    {
        public Book_lib()
        {
            InitializeComponent();
            DataContext = new BookViewModel();
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateHome(this, UserRole.Librarian);
    }
}
