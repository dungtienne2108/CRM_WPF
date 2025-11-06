using CRM.UI.ViewModels.ContractManagement;
using System.Windows.Controls;
using System.Windows.Input;

namespace CRM.UI.Views.ContractManagement
{
    /// <summary>
    /// Interaction logic for ContractManagementView.xaml
    /// </summary>
    public partial class ContractManagementView : UserControl
    {
        private readonly ContractManagementViewModel _viewModel;

        public ContractManagementView(ContractManagementViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            Loaded += UserControl_Loaded;
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
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
    }
}
