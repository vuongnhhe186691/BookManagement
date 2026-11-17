using System.Windows;
using A_PRN212.Domain.Enums;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class BorrowRecord_admin : Window
    {
        public BorrowRecord_admin()
        {
            InitializeComponent();
            var viewModel = new BorrowRecordViewModel();
            viewModel.LoadRecords();
            DataContext = viewModel;
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateHome(this, UserRole.Admin);
    }
}
