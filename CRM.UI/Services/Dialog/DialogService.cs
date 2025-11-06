using CRM.UI.ViewModels.Base;
using System.Windows;

namespace CRM.UI.Services.Dialog
{
    public class DialogService : IDialogService
    {
        public async Task<bool> ShowConfirmationAsync(string message, string title = "Confirmation")
        {
            return await Task.Run(() =>
            {
                var result = MessageBox.Show(
                    message,
                    title,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                return result == MessageBoxResult.Yes;
            });
        }

        public async Task ShowAlertAsync(string message, string title = "Alert")
        {
            await Task.Run(() =>
            {
                MessageBox.Show(
                    message,
                    title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            });
        }

        public async Task ShowErrorAsync(string message, string title = "Error")
        {
            await Task.Run(() =>
            {
                MessageBox.Show(
                    message,
                    title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            });
        }

        public async Task<TResult> ShowDialogAsync<TViewModel, TResult>(object parameter = null)
            where TViewModel : DialogViewModelBase<TResult>
        {
            var viewModel = Activator.CreateInstance<TViewModel>();
            await viewModel.InitializeAsync(parameter);

            // In real implementation, you would show the actual dialog window
            // This is a simplified version
            var dialogWindow = new Window
            {
                Title = viewModel.Title,
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = System.Windows.Application.Current.MainWindow,
                DataContext = viewModel
            };

            var result = dialogWindow.ShowDialog();

            return result == true ? viewModel.Result : default(TResult);
        }

        public async Task<string> ShowInputAsync(string message, string title = "Input", string defaultValue = "")
        {
            // Simplified implementation - in production, create a proper input dialog
            return await Task.FromResult(defaultValue);
        }

        public async Task<string> ShowFileOpenDialogAsync(string filter = "All files (*.*)|*.*")
        {
            return await Task.Run(() =>
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = filter
                };

                return dialog.ShowDialog() == true ? dialog.FileName : null;
            });
        }

        public async Task<string> ShowFileSaveDialogAsync(string filter = "All files (*.*)|*.*", string defaultFileName = "")
        {
            return await Task.Run(() =>
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = filter,
                    FileName = defaultFileName
                };

                return dialog.ShowDialog() == true ? dialog.FileName : null;
            });
        }
    }
}
