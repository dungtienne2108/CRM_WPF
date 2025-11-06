using CRM.UI.ViewModels.OpportunityManagement;
using System.Windows;

namespace CRM.UI.Views.OpportunityManagement
{
    /// <summary>
    /// Interaction logic for AddOpportunityDialog.xaml
    /// </summary>
    public partial class AddOpportunityDialog : Window
    {
        private readonly AddOpportunityViewModel _viewModel;

        public AddOpportunityDialog(AddOpportunityViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
