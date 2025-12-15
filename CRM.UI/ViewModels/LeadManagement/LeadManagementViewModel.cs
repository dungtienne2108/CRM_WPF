using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Lead;
using CRM.Application.Interfaces.Leads;
using CRM.Infrastructure.Services;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.LeadManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.LeadManagement
{
    public partial class LeadManagementViewModel : ViewModelBase
    {
        private readonly ILeadService _leadService;
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource? _cts;

        [ObservableProperty]
        private ObservableCollection<LeadItemViewModel> _leadItems = new();
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OpenDetailCommand))]
        [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
        private LeadItemViewModel _selectedLeadItem;

        public bool CanConvertStage => !(SelectedLeadItem != null && SelectedLeadItem.StatusId != 3);

        //[ObservableProperty]
        //private ObservableCollection<LeadItemViewModel> _filteredLeadItems = new();
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

        public LeadManagementViewModel(ILeadService leadService, IServiceProvider serviceProvider)
        {
            _leadService = leadService;

            CurrentPage = 1;
            RecordsPerPage = 25;
            TotalRecords = 0;
            _serviceProvider = serviceProvider;
        }

        public ObservableCollection<StatusOption> StatusOptions { get; } = new()
         {
             new StatusOption { Id = 1, Name = "Mới" },
             new StatusOption { Id = 2, Name = "Đã liện hệ" },
             new StatusOption { Id = 3, Name = "Chuyển đổi" },
             new StatusOption { Id = 4, Name = "Loại bỏ" },
         };

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
            var addLeadViewModel = _serviceProvider.GetRequiredService<AddLeadViewModel>();
            await addLeadViewModel.LoadDataAsync();
            var window = Window.GetWindow(System.Windows.Application.Current.MainWindow);
            var newLead = AddLeadDialog.ShowAddDialog(Window.GetWindow(window), addLeadViewModel);

            if (newLead != null)
            {
                await InitializeAsync();
                MessageBox.Show($"Đã thêm thành công khách hàng {newLead.Name}",
                    "Thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task Export()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Xuất danh sách khách hàng tiềm năng",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "LeadList.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialog.ShowDialog() == true)
            {
                var exportPath = dialog.FileName;
                var exportList = await _leadService.GetAllLeadsAsync(new()
                {
                    PageNumber = 1,
                    PageSize = int.MaxValue
                });
                ExcelHelper.ExportToExcelFile(exportList.Items, exportPath, "Leads");

                MessageBox.Show("Xuất file Excel thành công!", "Thành công",
            MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task ChangeStatus(object? parameter)
        {
            if (parameter is object[] parameters && parameters.Length == 2)
            {
                if (parameters[0] is LeadItemViewModel leadItem && parameters[1] is int newStatusId)
                {
                    await leadItem.UpdateStatus(newStatusId);
                    StatusChanged?.Invoke(leadItem.LeadDto, newStatusId);
                }
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
        private async Task OpenDetailAsync(LeadItemViewModel leadItem)
        {
            if (leadItem == null)
            {
                return;
            }

            var detailViewModel = new LeadDetailViewModel(_leadService, _serviceProvider);
            await detailViewModel.LoadDataAsync(leadItem);
            var detailWindow = new LeadDetailView(detailViewModel);
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
                var getLeadsRequest = new GetLeadRequest
                {
                    Keyword = SearchText,
                    PageNumber = CurrentPage,
                    PageSize = RecordsPerPage
                };

                var leadDatas = await _leadService.GetAllLeadsAsync(getLeadsRequest);

                LeadItems.Clear();
                int index = (CurrentPage - 1) * RecordsPerPage + 1;

                foreach (var lead in leadDatas.Items)
                {
                    LeadItems.Add(new LeadItemViewModel(lead, index++, _leadService, _serviceProvider));
                }

                TotalRecords = leadDatas.TotalCount;
                TotalRecordsText = $"Tổng số: {TotalRecords} bản ghi";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }

        }

        public List<LeadDto> GetSelectedLeads()
        {
            return LeadItems.Where(x => x.IsSelected).Select(x => x.LeadDto).ToList();
        }

        public List<LeadDto> GetAllLeads()
        {
            return LeadItems.Select(x => x.LeadDto).ToList();
        }

        public async Task RefreshDataAsync()
        {
            await _leadService.GetAllLeadsAsync(new GetLeadRequest
            {
                Keyword = SearchText,
                PageNumber = CurrentPage,
                PageSize = RecordsPerPage
            });
        }

        #endregion

        #region Events

        public event Action? CreateNewRequested;
        public event Action? ConvertStageRequested;
        public event Action<LeadDto, int>? StatusChanged;
        public event Action<List<LeadDto>>? LeadsRemoved;

        #endregion

        #region Property Changed Callbacks

        partial void OnCurrentPageChanged(int value)
        {
            FirstPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
            NextPageCommand.NotifyCanExecuteChanged();
            LastPageCommand.NotifyCanExecuteChanged();
        }

        partial void OnRecordsPerPageChanged(int value)
        {
            CurrentPage = 1;
            _ = InitializeAsync();
        }

        partial void OnTotalRecordsChanged(int value)
        {
            OnPropertyChanged(nameof(TotalPages));
            FirstPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
            NextPageCommand.NotifyCanExecuteChanged();
            LastPageCommand.NotifyCanExecuteChanged();
        }

        partial void OnCurrentPageChanged(int oldValue, int newValue)
        {
            if (newValue < 1)
            {
                CurrentPage = 1;
            }
            else if (TotalPages > 0 && newValue > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }
        #endregion
    }
}