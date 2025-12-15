using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Customer;
using CRM.Application.Interfaces.Customers;
using CRM.Infrastructure.Services;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.CustomerManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.CustomerManagement
{
    public partial class CustomerManagementViewModel : ViewModelBase
    {
        private readonly ICustomerService _customerService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private ObservableCollection<CustomerItemViewModel> _customerItems = new();
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OpenDetailCommand))]
        private CustomerItemViewModel? _selectedCustomerItem;
        [ObservableProperty]
        private string searchText = string.Empty;
        [ObservableProperty]
        private int currentPage;
        [ObservableProperty]
        private int recordsPerPage;
        [ObservableProperty]
        private int totalRecords;
        [ObservableProperty]
        private string totalRecordsText = string.Empty;
        [ObservableProperty]
        private bool isLoading;

        public CustomerManagementViewModel(
            ICustomerService customerService, IServiceProvider serviceProvider)
        {
            _customerService = customerService;

            CurrentPage = 1;
            RecordsPerPage = 25;
            TotalRecords = 0;
            _serviceProvider = serviceProvider;
        }

        [ObservableProperty]
        public ObservableCollection<int> _recordsPerPageOptions = new() { 10, 25, 50, 100 };

        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / RecordsPerPage);

        #region Commands
        [RelayCommand]
        private async Task SearchAsync()
        {
            CurrentPage = 1;
            await InitializeAsync();
        }

        [RelayCommand]
        private async Task CreateNew()
        {
            var addCustomerViewModel = _serviceProvider.GetRequiredService<AddCustomerViewModel>();
            await addCustomerViewModel.LoadDataAsync();
            var addCustomerDialog = new AddCustomerDialog(addCustomerViewModel);
            addCustomerDialog.ShowDialog();
            await InitializeAsync();
        }

        [RelayCommand]
        private async Task Export()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Xuất danh sách khách hàng",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "CustomerList.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialog.ShowDialog() == true)
            {
                var exportPath = dialog.FileName;
                var exportList = await _customerService.GetAllCustomersAsync(new()
                {
                    PageNumber = 1,
                    PageSize = int.MaxValue
                });
                ExcelHelper.ExportToExcelFile(exportList.Items, exportPath, "Customers");

                MessageBox.Show("Xuất file Excel thành công!", "Thành công",
            MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task ChangeRecordsPerPage(int newRecordsPerPage)
        {
            RecordsPerPage = newRecordsPerPage;
            CurrentPage = 1;
            await InitializeAsync();
        }

        // Pagination
        [RelayCommand(CanExecute = nameof(CanExecuteFirstPage))]
        private async Task FirstPage()
        {
            CurrentPage = 1;
            await InitializeAsync();
        }
        private bool CanExecuteFirstPage()
        {
            return CurrentPage > 1;
        }

        [RelayCommand(CanExecute = nameof(CanExecutePreviousPage))]
        private async Task PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await InitializeAsync();
            }
        }
        private bool CanExecutePreviousPage()
        {
            return CurrentPage > 1;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteNextPage))]
        private async Task NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await InitializeAsync();
            }
        }
        private bool CanExecuteNextPage()
        {
            return CurrentPage < TotalPages;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteLastPage))]
        private async Task LastPage()
        {
            CurrentPage = TotalPages;
            await InitializeAsync();
        }
        private bool CanExecuteLastPage()
        {
            return CurrentPage < TotalPages;
        }

        [RelayCommand]
        private async Task GoToPage(int pageNumber)
        {
            if (pageNumber >= 1 && pageNumber <= TotalPages)
            {
                CurrentPage = pageNumber;
                await InitializeAsync();
            }
            else
            {
                CurrentPage = 1;
                await InitializeAsync();
            }
        }

        [RelayCommand]
        private async Task OpenDetailAsync(CustomerItemViewModel customerItem)
        {
            if (customerItem == null)
            {
                return;
            }

            var detailViewModel = _serviceProvider.GetRequiredService<CustomerDetailViewModel>();
            await detailViewModel.LoadDataAsync(customerItem);
            var detailWindow = new CustomerDetailView(detailViewModel);
            //detailWindow.Owner = System.Windows.Application.Current.MainWindow;
            detailWindow.ShowDialog();
            await InitializeAsync();
        }

        #endregion

        #region Public Methods

        public async Task InitializeAsync()
        {
            IsLoading = true;
            try
            {
                var getCustomersRequest = new GetCustomerRequest
                {
                    Keyword = SearchText,
                    PageNumber = CurrentPage,
                    PageSize = RecordsPerPage
                };

                var customers = await _customerService.GetAllCustomersAsync(getCustomersRequest);

                CustomerItems.Clear();
                int index = (CurrentPage - 1) * RecordsPerPage + 1;

                foreach (var lead in customers.Items)
                {
                    CustomerItems.Add(new CustomerItemViewModel(lead, index++));
                }

                TotalRecords = customers.TotalCount;
                TotalRecordsText = $"Tổng số: {TotalRecords} bản ghi";
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Property changed
        partial void OnCurrentPageChanged(int value)
        {
            if (value < 1)
                CurrentPage = 1;
            else if (TotalPages > 0 && value > TotalPages)
                CurrentPage = TotalPages;

            FirstPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
            NextPageCommand.NotifyCanExecuteChanged();
            LastPageCommand.NotifyCanExecuteChanged();
        }

        partial void OnRecordsPerPageChanged(int value)
        {
            CurrentPage = 1;
            _ = InitializeAsync();

            FirstPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
            NextPageCommand.NotifyCanExecuteChanged();
            LastPageCommand.NotifyCanExecuteChanged();
        }

        partial void OnTotalRecordsChanged(int value)
        {
            OnPropertyChanged(nameof(TotalPages));
            FirstPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
            NextPageCommand.NotifyCanExecuteChanged();
            LastPageCommand.NotifyCanExecuteChanged();
        }
        #endregion

        #region Events

        public event Action? CreateNewRequested;
        public event Action<List<CustomerDto>>? CustomersRemoved;

        #endregion
    }
}
