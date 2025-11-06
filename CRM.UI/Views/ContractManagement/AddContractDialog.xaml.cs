using CRM.UI.ViewModels.ContractManagement;
using System.Windows;

namespace CRM.UI.Views.ContractManagement
{
    /// <summary>
    /// Interaction logic for AddContractDialog.xaml
    /// </summary>
    public partial class AddContractDialog : Window
    {
        private readonly AddContractViewModel _viewModel;
        public AddContractDialog(AddContractViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
