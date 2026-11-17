using System.Windows;

namespace A_PRN212.MVVM
{
    public interface IDialogService
    {
        void ShowInfo(string message, string title = "Notification");
        void ShowWarning(string message, string title = "Warning");
        void ShowError(string message, string title = "Error");
    }

    public sealed class WpfDialogService : IDialogService
    {
        private static readonly Lazy<WpfDialogService> _instance = new(() => new WpfDialogService());
        public static WpfDialogService Instance => _instance.Value;

        private WpfDialogService()
        {
        }

        public void ShowInfo(string message, string title = "Notification")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowWarning(string message, string title = "Warning")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void ShowError(string message, string title = "Error")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
