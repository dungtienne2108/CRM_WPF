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
    public partial class PaymentManagementViewModel : ViewModelBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IServiceProvider _serviceProvider;

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
        private ObservableCollection<PaymentItemViewModel> _paymentItems = new();
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OpenDetailCommand))]
        private PaymentItemViewModel _selectedPaymentItem;

        public PaymentManagementViewModel(IPaymentService paymentService, IServiceProvider serviceProvider)
        {
            _paymentService = paymentService;
            _serviceProvider = serviceProvider;

            CurrentPage = 1;
            RecordsPerPage = 25;
            TotalRecords = 0;
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
        private async Task Export()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Xuất danh sách thanh toán",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "PaymentList.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialog.ShowDialog() == true)
            {
                var exportPath = dialog.FileName;

                var exportList = await _paymentService.GetPaymentsAsync(new GetPaymentRequest
                {
                    PageNumber = 1,
                    PageSize = int.MaxValue
                });

                ExcelHelper.ExportToExcelFile(exportList.Items, exportPath, "Payments");

                MessageBox.Show("Xuất file Excel thành công!", "Thành công",
            MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task CreateNewAsync()
        {
            var addPaymentViewModel = _serviceProvider.GetRequiredService<AddPaymentViewModel>();
            await addPaymentViewModel.LoadDataAsync();
            var addPaymentDialog = new AddPaymentDialog(addPaymentViewModel);
            System.Windows.Application.Current.MainWindow = addPaymentDialog;
            addPaymentDialog.ShowDialog();
            await InitializeAsync();
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
        private async Task OpenDetailAsync(PaymentItemViewModel paymentItem)
        {
            var paymentDetailViewModel = _serviceProvider.GetRequiredService<PaymentDetailViewModel>();
            await paymentDetailViewModel.LoadDataAsync(paymentItem);
            var paymentDetailView = new PaymentDetailView(paymentDetailViewModel);
            paymentDetailView.ShowDialog();
            await InitializeAsync();

        }
        #endregion

        #region Initialization
        public async Task InitializeAsync()
        {
            IsLoading = true;
            try
            {
                var getPaymentsRequest = new GetPaymentRequest
                {
                    Keyword = SearchText,
                    PageNumber = CurrentPage,
                    PageSize = RecordsPerPage
                };

                var paymentsResult = await _paymentService.GetPaymentsAsync(getPaymentsRequest);

                PaymentItems.Clear();
                int index = (CurrentPage - 1) * RecordsPerPage + 1;

                foreach (var payment in paymentsResult.Items)
                {
                    PaymentItems.Add(new PaymentItemViewModel(payment, index++));
                }

                TotalRecords = paymentsResult.TotalCount;
                TotalRecordsText = $"Tổng số bản ghi: {TotalRecords}";

                FirstPageCommand.NotifyCanExecuteChanged();
                PreviousPageCommand.NotifyCanExecuteChanged();
                NextPageCommand.NotifyCanExecuteChanged();
                LastPageCommand.NotifyCanExecuteChanged();
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
