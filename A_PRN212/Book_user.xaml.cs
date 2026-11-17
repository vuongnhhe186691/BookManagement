using System.Windows;
using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Enums;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class Book_user : Window
    {
        private readonly BookViewModel _viewModel;

        public Book_user()
            : this(UserSession.Instance.CurrentUser)
        {
        }

        public Book_user(User? user)
        {
            InitializeComponent();
            var currentUser = user ?? UserSession.Instance.CurrentUser;
            _viewModel = new BookViewModel(currentUser);
            _viewModel.BorrowSucceeded += record =>
            {
                var borrowRecordPage = new BorrowRecord_user(record.BorrowDate, record.DueDate, record.Status);
                NavigationHelper.NavigateTo(this, borrowRecordPage);
            };
            DataContext = _viewModel;
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateHome(this, UserRole.Reader);
    }
}
