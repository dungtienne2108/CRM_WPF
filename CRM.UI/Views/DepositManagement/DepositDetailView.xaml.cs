using CRM.UI.ViewModels.DepositManagement;
using System.Windows;

namespace CRM.UI.Views.DepositManagement
{
    /// <summary>
    /// Interaction logic for DepositDetailView.xaml
    /// </summary>
    public partial class DepositDetailView : Window
    {
        private readonly DepositDetailViewModel _viewModel;

        public DepositDetailView(DepositDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
