using CRM.UI.ViewModels.ContactManagement;
using System.Windows;

namespace CRM.UI.Views.ContactManagement
{
    /// <summary>
    /// Interaction logic for AddContactDialog.xaml
    /// </summary>
    public partial class AddContactDialog : Window
    {
        private readonly AddContactViewModel _viewModel;

        public AddContactDialog(AddContactViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
