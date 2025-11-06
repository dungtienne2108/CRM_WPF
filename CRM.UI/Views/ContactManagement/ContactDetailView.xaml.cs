using CRM.UI.ViewModels.ContactManagement;
using System.Windows;

namespace CRM.UI.Views.ContactManagement
{
    /// <summary>
    /// Interaction logic for ContactDetailView.xaml
    /// </summary>
    public partial class ContactDetailView : Window
    {
        private readonly ContactDetailViewModel _viewModel;

        public ContactDetailView(ContactDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
