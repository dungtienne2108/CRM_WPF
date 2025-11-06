using CRM.UI.ViewModels.LeadManagement;
using System.Windows;

namespace CRM.UI.Views.LeadManagement
{
    /// <summary>
    /// Interaction logic for LeadDetailView.xaml
    /// </summary>
    public partial class LeadDetailView : Window
    {
        private readonly LeadDetailViewModel _viewModel;

        public LeadDetailView(LeadDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
