using System.Windows;
using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Enums;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class Borrow_userlist : Window
    {
        public Borrow_userlist()
            : this(UserSession.Instance.CurrentUser)
        {
        }

        public Borrow_userlist(User? user)
        {
            InitializeComponent();
            var currentUser = user ?? UserSession.Instance.CurrentUser;
            var viewModel = new BorrowRecordViewModel(currentUser?.UserId);
            if (currentUser != null)
            {
                viewModel.LoadRecords();
            }
            DataContext = viewModel;
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateHome(this, UserRole.Reader);
    }
}
