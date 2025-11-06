using CRM.UI.ViewModels.OpportunityManagement;
using System.Windows;

namespace CRM.UI.Views.OpportunityManagement
{
    /// <summary>
    /// Interaction logic for OpportunityDetail.xaml
    /// </summary>
    public partial class OpportunityDetail : Window
    {
        private readonly OpportunityDetailViewModel _viewModel;

        public OpportunityDetail(OpportunityDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
