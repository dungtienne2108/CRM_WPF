using CRM.UI.ViewModels.Admin.ProjectManagement;
using System.Windows;

namespace CRM.UI.Views.Admin.ProjectManagement
{
    /// <summary>
    /// Interaction logic for AddProductDialog.xaml
    /// </summary>
    public partial class AddProductDialog : Window
    {
        private readonly AddProductViewModel _viewModel;

        public AddProductDialog(AddProductViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
