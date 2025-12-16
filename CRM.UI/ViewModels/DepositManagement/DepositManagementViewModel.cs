using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Deposit;
using CRM.Application.Interfaces.Deposit;
using CRM.Infrastructure.Services;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.DepositManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.DepositManagement
{
    public partial class DepositManagementViewModel : ViewModelBase
    {
        private readonly IDepositService _depositService;
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
        private ObservableCollection<DepositItemViewModel> _depositItems = new();
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OpenDetailCommand))]
        private DepositItemViewModel? _selectedDepositItem;

        public DepositManagementViewModel(IDepositService depositService, IServiceProvider serviceProvider)
        {
            _depositService = depositService;
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
        private async Task CreateNewAsync()
        {
            var addDepositViewModel = _serviceProvider.GetRequiredService<AddDepositViewModel>();
            await addDepositViewModel.InitializeAsync();
            var addDepositView = new AddDepositDialog(addDepositViewModel);
            var res = addDepositView.ShowDialog();
            await InitializeAsync();

        }

        [RelayCommand]
        private async Task Export()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Xuất danh sách đặt cọc",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "DepositList.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialog.ShowDialog() == true)
            {
                var exportPath = dialog.FileName;
                var exportList = await _depositService.GetDepositsAsync(new()
                {
                    PageSize = int.MaxValue,
                    PageNumber = 1
                });
                ExcelHelper.ExportToExcelFile(exportList.Items, exportPath, "Deposits");

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
        private async Task OpenDetailAsync()
        {
            var depositDetailViewModel = _serviceProvider.GetRequiredService<DepositDetailViewModel>();
            if (SelectedDepositItem != null)
            {
                await depositDetailViewModel.LoadDataAsync(SelectedDepositItem);
                var depositDetailView = new DepositDetailView(depositDetailViewModel);
                depositDetailView.ShowDialog();
                await InitializeAsync();
            }
        }
        #endregion

        #region Initialization
        public async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                IsBusy = true;

                var request = new GetDepositRequest
                {
                    Keyword = SearchText,
                    PageNumber = CurrentPage,
                    PageSize = RecordsPerPage
                };

                var pagedResult = await _depositService.GetDepositsAsync(request);
                var index = (CurrentPage - 1) * RecordsPerPage + 1;

                DepositItems.Clear();

                foreach (var deposit in pagedResult.Items)
                {
                    var itemViewModel = new DepositItemViewModel(deposit, index++);
                    DepositItems.Add(itemViewModel);
                }

                TotalRecords = pagedResult.TotalCount;
                TotalRecordsText = $" Tổng : {TotalRecords} bản ghi";

                FirstPageCommand.NotifyCanExecuteChanged();
                PreviousPageCommand.NotifyCanExecuteChanged();
                NextPageCommand.NotifyCanExecuteChanged();
                LastPageCommand.NotifyCanExecuteChanged();

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasCustomErrors = true;
            }
            finally
            {
                IsLoading = false;
                IsBusy = false;
            }
        }
        #endregion
    }
}
