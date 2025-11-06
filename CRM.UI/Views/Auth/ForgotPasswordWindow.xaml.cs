using CRM.UI.ViewModels.Auth;
using System.Windows;

namespace CRM.UI.Views.Auth
{
    /// <summary>
    /// Interaction logic for ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ForgotPasswordWindow : Window
    {
        private readonly ForgotPasswordViewModel _viewModel;

        public ForgotPasswordWindow(ForgotPasswordViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            EmailTextBox.Focus();
            EmailTextBox.SelectAll();
        }
    }
}
