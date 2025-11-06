using CRM.UI.ViewModels.DepositManagement;
using System.Windows;

namespace CRM.UI.Views.DepositManagement
{
    /// <summary>
    /// Interaction logic for AddDepositDialog.xaml
    /// </summary>
    public partial class AddDepositDialog : Window
    {
        private readonly AddDepositViewModel _viewModel;

        public AddDepositDialog(AddDepositViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
