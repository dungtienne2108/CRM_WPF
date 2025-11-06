using CRM.UI.ViewModels.PaymentManagement;
using System.Windows;

namespace CRM.UI.Views.PaymentManagement
{
    /// <summary>
    /// Interaction logic for InvoiceDetailView.xaml
    /// </summary>
    public partial class InvoiceDetailView : Window
    {
        private readonly InvoiceDetailViewModel _viewModel;

        public InvoiceDetailView(InvoiceDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
