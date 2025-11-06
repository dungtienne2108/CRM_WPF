using CRM.UI.ViewModels.PaymentManagement;
using System.Windows;

namespace CRM.UI.Views.PaymentManagement
{
    /// <summary>
    /// Interaction logic for AddInvoiceDialog.xaml
    /// </summary>
    public partial class AddInvoiceDialog : Window
    {
        private readonly AddInvoiceViewModel _viewModel;

        public AddInvoiceDialog(AddInvoiceViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
