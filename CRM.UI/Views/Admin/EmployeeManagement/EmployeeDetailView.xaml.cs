using CRM.UI.ViewModels.Admin.EmployeeManagement;
using System.Windows;

namespace CRM.UI.Views.Admin.EmployeeManagement
{
    /// <summary>
    /// Interaction logic for EmployeeDetailView.xaml
    /// </summary>
    public partial class EmployeeDetailView : Window
    {
        private readonly EmployeeDetailViewModel _viewModel;

        public EmployeeDetailView(EmployeeDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
