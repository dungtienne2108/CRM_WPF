using CRM.UI.ViewModels.PaymentManagement;
using System.Windows;

namespace CRM.UI.Views.ContractManagement
{
    /// <summary>
    /// Interaction logic for AddPaymentScheduleDialog.xaml
    /// </summary>
    public partial class AddPaymentScheduleDialog : Window
    {
        private readonly AddPaymentScheduleViewModel _viewModel;

        public AddPaymentScheduleDialog(AddPaymentScheduleViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
