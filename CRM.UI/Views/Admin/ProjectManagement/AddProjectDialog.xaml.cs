using CRM.UI.ViewModels.Admin.ProjectManagement;
using System.Windows;

namespace CRM.UI.Views.Admin.ProjectManagement
{
    /// <summary>
    /// Interaction logic for AddProjectDialog.xaml
    /// </summary>
    public partial class AddProjectDialog : Window
    {
        private readonly AddProjectViewModel _viewModel;

        public AddProjectDialog(AddProjectViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
