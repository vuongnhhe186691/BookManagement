using System.Windows;
using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Enums;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class BorrowRecord_user : Window
    {
        public BorrowRecord_user()
            : this(UserSession.Instance.CurrentUser)
        {
        }

        public BorrowRecord_user(User? user)
        {
            InitializeComponent();
            var currentUser = user ?? UserSession.Instance.CurrentUser;
            var viewModel = new BorrowRecordViewModel(currentUser?.UserId);
            if (currentUser != null)
            {
                viewModel.LoadLatestForUser(currentUser.UserId);
            }
            DataContext = viewModel;
        }

        public BorrowRecord_user(DateTime borrowDate, DateTime dueDate, string status)
        {
            InitializeComponent();
            var viewModel = new BorrowRecordViewModel();
            viewModel.SetDirectPreview(borrowDate, dueDate, status);
            DataContext = viewModel;
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateHome(this, UserRole.Reader);
    }
}
