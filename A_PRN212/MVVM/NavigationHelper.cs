using System.Windows;
using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Enums;

namespace A_PRN212.MVVM
{
    /// <summary>
    /// Resolves and navigates to role-specific application windows.
    /// </summary>
    public static class NavigationHelper
    {
        public static Window CreateHomeWindow(User? user)
        {
            string role = user?.Role ?? UserSession.Instance.CurrentUser?.Role ?? string.Empty;

            return role switch
            {
                UserRole.Admin => new Home_admin(),
                UserRole.Librarian => new Home_lib(),
                UserRole.Reader => user != null ? new Home_user(user) : new Home_user(),
                _ => new Login()
            };
        }

        public static void NavigateTo(Window currentWindow, Window targetWindow)
        {
            targetWindow.Show();
            currentWindow.Close();
        }

        public static void NavigateHome(Window currentWindow, string? fallbackRole = null)
        {
            var currentUser = UserSession.Instance.CurrentUser;
            if (currentUser != null)
            {
                NavigateTo(currentWindow, CreateHomeWindow(currentUser));
                return;
            }

            Window fallback = fallbackRole switch
            {
                UserRole.Admin => new Home_admin(),
                UserRole.Librarian => new Home_lib(),
                UserRole.Reader => new Home_user(),
                _ => new Login()
            };
            NavigateTo(currentWindow, fallback);
        }
    }
}
