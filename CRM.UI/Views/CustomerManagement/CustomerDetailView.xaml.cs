using CRM.UI.ViewModels.CustomerManagement;
using System.Windows;

namespace CRM.UI.Views.CustomerManagement
{
    /// <summary>
    /// Interaction logic for CustomerDetailView.xaml
    /// </summary>
    public partial class CustomerDetailView : Window
    {
        private readonly CustomerDetailViewModel _viewModel;

        public CustomerDetailView(CustomerDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
