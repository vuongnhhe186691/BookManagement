using System.Windows;
using A_PRN212.Domain.Enums;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class User_lib : Window
    {
        public User_lib()
        {
            InitializeComponent();
            DataContext = new UserViewModel(isAdminMode: false);
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateHome(this, UserRole.Librarian);
    }
}
