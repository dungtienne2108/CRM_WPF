using CRM.UI.ViewModels.Admin.EmployeeManagement;
using System.Windows;

namespace CRM.UI.Views.Admin.EmployeeManagement
{
    /// <summary>
    /// Interaction logic for AddEmployeeView.xaml
    /// </summary>
    public partial class AddEmployeeView : Window
    {
        private readonly AddEmployeeViewModel _viewModel;

        public AddEmployeeView(AddEmployeeViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
