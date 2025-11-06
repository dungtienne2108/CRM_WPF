using CRM.UI.ViewModels.Auth;
using System.Windows;
using System.Windows.Controls;

namespace CRM.UI.Views.Auth
{
    /// <summary>
    /// Interaction logic for ResetPasswordWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow : Window
    {
        private readonly ResetPasswordViewModel _viewModel;

        public ResetPasswordWindow(ResetPasswordViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ResetPasswordViewModel vm)
            {
                vm.NewPassword = (sender as PasswordBox)?.Password ?? string.Empty;
            }
        }
    }
}
