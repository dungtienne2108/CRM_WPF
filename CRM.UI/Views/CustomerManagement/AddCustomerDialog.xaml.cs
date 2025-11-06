using CRM.UI.ViewModels.CustomerManagement;
using System.Windows;

namespace CRM.UI.Views.CustomerManagement
{
    /// <summary>
    /// Interaction logic for AddCustomerDialog.xaml
    /// </summary>
    public partial class AddCustomerDialog : Window
    {
        private readonly AddCustomerViewModel _viewModel;

        public AddCustomerDialog(AddCustomerViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
