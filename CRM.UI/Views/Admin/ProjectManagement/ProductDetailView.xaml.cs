using CRM.UI.ViewModels.Admin.ProjectManagement;
using System.Windows;

namespace CRM.UI.Views.Admin.ProjectManagement
{
    /// <summary>
    /// Interaction logic for ProductDetailView.xaml
    /// </summary>
    public partial class ProductDetailView : Window
    {
        private readonly ProductDetailViewModel _viewModel;

        public ProductDetailView(ProductDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
