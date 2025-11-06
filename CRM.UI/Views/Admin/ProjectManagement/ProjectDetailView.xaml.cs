using CRM.UI.ViewModels.Admin.ProjectManagement;
using System.Windows;

namespace CRM.UI.Views.Admin.ProjectManagement
{
    /// <summary>
    /// Interaction logic for ProjectDetailView.xaml
    /// </summary>
    public partial class ProjectDetailView : Window
    {
        private readonly ProjectDetailViewModel _viewModel;

        public ProjectDetailView(ProjectDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
