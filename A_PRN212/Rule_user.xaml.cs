using System.Windows;
using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Enums;
using A_PRN212.MVVM;
using A_PRN212.ViewModels;

namespace A_PRN212
{
    public partial class Rule_user : Window
    {
        public Rule_user()
            : this(UserSession.Instance.CurrentUser)
        {
        }

        public Rule_user(User? user)
        {
            InitializeComponent();
            if (user != null)
            {
                UserSession.Instance.SetUser(user);
            }
            DataContext = new LibraryRuleViewModel();
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.NavigateHome(this, UserRole.Reader);
    }
}
