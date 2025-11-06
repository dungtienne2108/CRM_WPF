using CRM.UI.ViewModels.OpportunityManagement;
using System.Windows;

namespace CRM.UI.Views.OpportunityManagement
{
    /// <summary>
    /// Interaction logic for AddProductToOpportunityDialog.xaml
    /// </summary>
    public partial class AddProductToOpportunityDialog : Window
    {
        private readonly AddProductToOpportunityViewModel _viewModel;

        public AddProductToOpportunityDialog(AddProductToOpportunityViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
