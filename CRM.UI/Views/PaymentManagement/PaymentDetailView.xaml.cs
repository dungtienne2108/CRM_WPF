using CRM.UI.ViewModels.PaymentManagement;
using System.Windows;

namespace CRM.UI.Views.PaymentManagement
{
    /// <summary>
    /// Interaction logic for PaymentDetailView.xaml
    /// </summary>
    public partial class PaymentDetailView : Window
    {
        private readonly PaymentDetailViewModel _viewModel;

        public PaymentDetailView(PaymentDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
