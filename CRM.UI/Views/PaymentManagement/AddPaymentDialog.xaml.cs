using CRM.UI.ViewModels.PaymentManagement;
using System.Windows;

namespace CRM.UI.Views.PaymentManagement
{
    /// <summary>
    /// Interaction logic for AddPaymentDialog.xaml
    /// </summary>
    public partial class AddPaymentDialog : Window
    {
        private readonly AddPaymentViewModel _viewModel;

        public AddPaymentDialog(AddPaymentViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
