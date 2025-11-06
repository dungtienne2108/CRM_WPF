using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Opportunity;
using CRM.Application.Interfaces.Opportunity;
using CRM.Infrastructure.Services;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.OpportunityManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.OpportunityManagement
{
    public partial class OpportunityManagementViewModel : ViewModelBase
    {
        private readonly IOpportunityService _opportunityService;
        private readonly IServiceProvider _serviceProvider;

        public OpportunityManagementViewModel(IOpportunityService opportunityService, IServiceProvider serviceProvider)
        {
            _opportunityService = opportunityService;
            CurrentPage = 1;
            RecordsPerPage = RecordsPerPageOptions.First();
            TotalRecords = 0;
            _serviceProvider = serviceProvider;
        }

        [ObservableProperty]
        private ObservableCollection<OpportunityItemViewModel> _opportunities = new();
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OpenDetailCommand))]
        [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
        private OpportunityItemViewModel _selectedOpportunity;
        [ObservableProperty]
        private string searchText = string.Empty;
        [ObservableProperty]
        private int stageId;
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

        public static readonly List<int> RecordsPerPageOptions = new() { 10, 25, 50, 100 };

        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / RecordsPerPage);

        #region Commands
        [RelayCommand]
        private async Task CreateNewAsync()
        {
            var addOpportunityViewModel = _serviceProvider.GetRequiredService<AddOpportunityViewModel>();
            await addOpportunityViewModel.LoadDataAsync();
            var addOpportunityWindow = new AddOpportunityDialog(addOpportunityViewModel);
            addOpportunityWindow.ShowDialog();
            await InitializeAsync();
        }

        [RelayCommand]
        private async Task Export()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Xuất danh sách cơ hội",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "OpportunityList.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialog.ShowDialog() == true)
            {
                var exportPath = dialog.FileName;
                var exportList = await _opportunityService.GetAllOpportunitiesAsync(new()
                {
                    PageNumber = 1,
                    PageSize = 1000
                });
                ExcelHelper.ExportToExcelFile(exportList.Items, exportPath, "Opportunities");

                MessageBox.Show("Xuất file Excel thành công!", "Thành công",
            MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        [RelayCommand]
        private async Task SearchAsync()
        {
            CurrentPage = 1;
            await InitializeAsync();
        }

        [RelayCommand]
        private async Task FilterByStageAsync(object stageId)
        {

            if (stageId == null || !int.TryParse(stageId.ToString(), out int stageIdValue))
            {
                StageId = 0;
                await InitializeAsync();
                return;
            }

            StageId = stageIdValue;

            switch (StageId)
            {
                case 0:
                    StageId = 0; // All
                    await InitializeAsync();
                    break;
                case 1:
                    StageId = 1;
                    await InitializeAsync();
                    break;
                case 2:
                    StageId = 2;
                    await InitializeAsync();
                    break;
                case 3:
                    StageId = 3;
                    await InitializeAsync();
                    break;
                case 4:
                    StageId = 4;
                    await InitializeAsync();
                    break;
                case 5:
                    StageId = 5;
                    await InitializeAsync();
                    break;
                default:
                    StageId = 0;
                    await InitializeAsync();
                    break;
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
        private async Task OpenDetail(OpportunityItemViewModel opportunity)
        {
            if (opportunity == null)
            {
                return;
            }

            var detailViewModel = new OpportunityDetailViewModel(_opportunityService, _serviceProvider);
            await detailViewModel.LoadDataAsync(opportunity);
            var detailWindow = new OpportunityDetail(detailViewModel);
            detailWindow.Show();
        }

        #endregion

        #region Public Methods

        public async Task InitializeAsync()
        {
            IsLoading = true;
            try
            {
                var getOpportunityRequest = new GetOpportunityRequest
                {
                    PageNumber = CurrentPage,
                    PageSize = RecordsPerPage,
                    Keyword = SearchText,
                    OpportunityStageId = StageId > 0 ? StageId : null
                };

                var result = await _opportunityService.GetAllOpportunitiesAsync(getOpportunityRequest);
                Opportunities.Clear();
                int index = (CurrentPage - 1) * RecordsPerPage + 1;
                foreach (var opportunity in result.Items)
                {
                    Opportunities.Add(new OpportunityItemViewModel(index++, opportunity));
                }
                TotalRecords = result.TotalCount;
                TotalRecordsText = $"Tổng số cơ hội: {TotalRecords}";

                // Notify command can execute state changed
                FirstPageCommand.NotifyCanExecuteChanged();
                PreviousPageCommand.NotifyCanExecuteChanged();
                NextPageCommand.NotifyCanExecuteChanged();
                LastPageCommand.NotifyCanExecuteChanged();
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them)
                MessageBox.Show($"Error loading opportunities: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion


    }
}
