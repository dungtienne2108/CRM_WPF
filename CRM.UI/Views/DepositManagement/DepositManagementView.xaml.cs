using CRM.UI.ViewModels.DepositManagement;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CRM.UI.Views.DepositManagement
{
    /// <summary>
    /// Interaction logic for DepositManagementView.xaml
    /// </summary>
    public partial class DepositManagementView : UserControl
    {
        private readonly DepositManagementViewModel _viewModel;

        public DepositManagementView(DepositManagementViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            Loaded += UserControl_Loaded;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.InitializeAsync();
        }

        private void PageNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox textBox)
                {
                    var pageNumber = int.TryParse(textBox.Text, out int number) ? number : 1;
                    if (_viewModel.GoToPageCommand.CanExecute(pageNumber))
                    {
                        _viewModel.GoToPageCommand.Execute(pageNumber);
                        textBox.Text = _viewModel.CurrentPage.ToString();
                    }
                }
            }
        }

        private void PageNumberTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                var tb = (TextBox)sender;
                if (tb.Text.Length <= 1)
                {
                    e.Handled = true;
                }
            }
        }
    }
}
