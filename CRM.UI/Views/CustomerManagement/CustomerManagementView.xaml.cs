using CRM.Application.Dtos.Customer;
using CRM.UI.ViewModels.CustomerManagement;
using System.Windows;
using System.Windows.Controls;

namespace CRM.UI.Views.CustomerManagement
{
    /// <summary>
    /// Interaction logic for CustomerManagementView.xaml
    /// </summary>
    public partial class CustomerManagementView : UserControl
    {
        private readonly CustomerManagementViewModel _viewModel;
        public CustomerManagementView(
            CustomerManagementViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            Loaded += UserControl_Loaded;
            Unloaded += Cleanup;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.InitializeAsync();
            SubscribeToViewModelEvents();
        }

        private void Cleanup(object sender, RoutedEventArgs e)
        {
            _viewModel.CreateNewRequested -= OnCreateNewRequested;
            _viewModel.CustomersRemoved -= OnCustomersRemoved;
        }

        private void SubscribeToViewModelEvents()
        {
            _viewModel.CreateNewRequested += OnCreateNewRequested;
            _viewModel.CustomersRemoved += OnCustomersRemoved;
        }

        private void OnCreateNewRequested()
        {
            MessageBox.Show($"Đã thêm thành công khách hàng",
                    "Thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
        }

        private void OnCustomersRemoved(List<CustomerDto> removedCustomers)
        {
            MessageBox.Show($"Đã xóa thành công {removedCustomers.Count} khách hàng",
                    "Thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
        }
    }
}
