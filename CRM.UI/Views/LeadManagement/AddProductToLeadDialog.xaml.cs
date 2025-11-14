using CRM.UI.ViewModels.LeadManagement;
using System.Windows;

namespace CRM.UI.Views.LeadManagement
{
    /// <summary>
    /// Interaction logic for AddProductToLeadDialog.xaml
    /// </summary>
    public partial class AddProductToLeadDialog : Window
    {
        private readonly AddProductToLeadViewModel _viewModel;

        public AddProductToLeadDialog(AddProductToLeadViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
