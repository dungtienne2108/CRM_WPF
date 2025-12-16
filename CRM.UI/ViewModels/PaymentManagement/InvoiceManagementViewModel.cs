using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Payment;
using CRM.Application.Interfaces.Payment;
using CRM.Infrastructure.Services;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.PaymentManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.PaymentManagement
{
    public partial class InvoiceManagementViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPaymentService _paymentService;

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

        [ObservableProperty]
        private ObservableCollection<InvoiceItemViewModel> _invoiceItems = new();
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OpenDetailCommand))]
        private InvoiceItemViewModel? _selectedInvoiceItem;

        public InvoiceManagementViewModel(IServiceProvider serviceProvider, IPaymentService paymentService)
        {
            CurrentPage = 1;
            RecordsPerPage = 25;
            TotalRecords = 0;
            _serviceProvider = serviceProvider;
            _paymentService = paymentService;
        }

        public static readonly List<int> RecordsPerPageOptions = new() { 10, 25, 50, 100 };

        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / RecordsPerPage);

        #region Commands
        [RelayCommand]
        private async Task SearchAsync()
        {
            CurrentPage = 1;
            await InitializeAsync();
        }

        [RelayCommand]
        private async Task CreateNewAsync()
        {
            var addInvoiceViewModel = _serviceProvider.GetRequiredService<AddInvoiceViewModel>();
            await addInvoiceViewModel.LoadDataAsync();
            var addPaymentDialog = new AddInvoiceDialog(addInvoiceViewModel);
            addPaymentDialog.Owner = System.Windows.Application.Current.MainWindow;
            var res = addPaymentDialog.ShowDialog();

            await InitializeAsync();

        }

        [RelayCommand]
        private async Task Export()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Xuất danh sách hóa đơn",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "InvoiceList.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialog.ShowDialog() == true)
            {
                var exportPath = dialog.FileName;
                var exportList = await _paymentService.GetInvoicesAsync(new()
                {
                    PageNumber = 1,
                    PageSize = int.MaxValue
                });
                ExcelHelper.ExportToExcelFile(exportList.Items, exportPath, "Invoices");

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
        private async Task OpenDetailAsync(InvoiceItemViewModel invoiceItem)
        {
            var invoiceDetailViewModel = _serviceProvider.GetRequiredService<InvoiceDetailViewModel>();
            await invoiceDetailViewModel.LoadDataAsync(invoiceItem);
            var invoiceDetailView = new InvoiceDetailView(invoiceDetailViewModel);
            invoiceDetailView.ShowDialog();
            await InitializeAsync();
        }
        #endregion

        #region Initialization
        public async Task InitializeAsync()
        {
            IsLoading = true;
            try
            {
                var getInvoicesRequest = new GetInvoiceRequest
                {
                    Keyword = SearchText,
                    PageNumber = CurrentPage,
                    PageSize = RecordsPerPage
                };

                var result = await _paymentService.GetInvoicesAsync(getInvoicesRequest);

                InvoiceItems.Clear();
                int index = (CurrentPage - 1) * RecordsPerPage + 1;

                foreach (var invoice in result.Items)
                {
                    InvoiceItems.Add(new(invoice, index++));
                }

                TotalRecords = result.TotalCount;
                TotalRecordsText = $"Tổng số bản ghi: {TotalRecords}";

            }
            catch (Exception)
            {
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion
    }
}
