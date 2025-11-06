using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Employee;
using CRM.Application.Interfaces.Employee;
using CRM.Infrastructure.Services;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.Admin.EmployeeManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.Admin.EmployeeManagement
{
    public partial class EmployeeManagementViewModel : ViewModelBase
    {
        private readonly IEmployeeService _employeeService;
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
        private ObservableCollection<EmployeeItemViewModel> _employeeItems = new();
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OpenDetailCommand))]
        private EmployeeItemViewModel _selectedEmployeeItem;

        public EmployeeManagementViewModel(IEmployeeService employeeService, IServiceProvider serviceProvider)
        {
            _employeeService = employeeService;

            CurrentPage = 1;
            RecordsPerPage = 10;
            TotalRecords = 0;
            _serviceProvider = serviceProvider;
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
            var addEmployeeViewModel = _serviceProvider.GetRequiredService<AddEmployeeViewModel>();
            await addEmployeeViewModel.LoadDataAsync();
            var addEmployeeDialog = new AddEmployeeView(addEmployeeViewModel);
            var res = addEmployeeDialog.ShowDialog();


            await InitializeAsync();

        }

        [RelayCommand]
        private async Task Export()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Xuất danh sách nhân viên",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "EmployeeList.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialog.ShowDialog() == true)
            {
                var exportPath = dialog.FileName;
                var exportListResult = await _employeeService.GetAllEmployeesAsync(new()
                {
                    PageNumber = 1,
                    PageSize = int.MaxValue
                });

                ExcelHelper.ExportToExcelFile(exportListResult.Items, exportPath, "Employees");

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
        private async Task OpenDetailAsync(EmployeeItemViewModel employeeItem)
        {
            var employeeDetailViewModel = _serviceProvider.GetRequiredService<EmployeeDetailViewModel>();
            await employeeDetailViewModel.LoadDataAsync(employeeItem);
            var employeeDetailDialog = new EmployeeDetailView(employeeDetailViewModel);
            var res = employeeDetailDialog.ShowDialog();

            await InitializeAsync();

        }
        #endregion

        #region Initialization
        public async Task InitializeAsync()
        {
            IsLoading = true;
            try
            {
                var getEmployeesRequest = new GetEmployeeRequest
                {
                    Keyword = SearchText,
                    PageNumber = CurrentPage,
                    PageSize = RecordsPerPage
                };

                var pagedResult = await _employeeService.GetAllEmployeesAsync(getEmployeesRequest);

                EmployeeItems.Clear();

                int index = (CurrentPage - 1) * RecordsPerPage + 1;

                foreach (var employee in pagedResult.Items)
                {
                    EmployeeItems.Add(new EmployeeItemViewModel(employee, index++));
                }

                TotalRecords = pagedResult.TotalCount;
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
