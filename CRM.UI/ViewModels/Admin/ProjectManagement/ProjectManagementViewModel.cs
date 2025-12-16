using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Project;
using CRM.Application.Interfaces.Project;
using CRM.Infrastructure.Services;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.Admin.ProjectManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.Admin.ProjectManagement
{
    public partial class ProjectManagementViewModel : ViewModelBase
    {
        private readonly IProjectService _projectService;
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
        private ObservableCollection<ProjectItemViewModel> _projectItems = new();
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OpenDetailCommand))]
        private ProjectItemViewModel? _selectedProjectItem;

        public ProjectManagementViewModel(IProjectService projectService, IServiceProvider serviceProvider)
        {
            _projectService = projectService;

            CurrentPage = 1;
            RecordsPerPage = 25;
            TotalRecords = 0;
            TotalRecordsText = "Tổng số bản ghi: 0";
            IsLoading = false;
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
            var addProjectViewModel = _serviceProvider.GetRequiredService<AddProjectViewModel>();
            var addProjectDialog = new AddProjectDialog(addProjectViewModel);
            var res = addProjectDialog.ShowDialog();

            await InitializeAsync();

        }

        [RelayCommand]
        private async Task Export()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Xuất danh sách dự án",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "ProjectList.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialog.ShowDialog() == true)
            {
                var exportPath = dialog.FileName;
                var exportListResult = await _projectService.GetProjectsAsync(new()
                {
                    PageSize = int.MaxValue,
                    PageNumber = 1
                });

                ExcelHelper.ExportToExcelFile(exportListResult.Items, exportPath, "Projects");

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
        private async Task OpenDetailAsync(ProjectItemViewModel projectItem)
        {
            var detailViewModel = _serviceProvider.GetRequiredService<ProjectDetailViewModel>();
            await detailViewModel.LoadDataAsync(projectItem.Id);
            var detailView = new Views.Admin.ProjectManagement.ProjectDetailView(detailViewModel);
            detailView.Show();
        }
        #endregion

        #region Initialization
        public async Task InitializeAsync()
        {
            IsLoading = true;
            try
            {
                var getProjectRequest = new GetProjectRequest
                {
                    Keyword = SearchText,
                    PageNumber = CurrentPage,
                    PageSize = RecordsPerPage
                };

                var pagedResult = await _projectService.GetProjectsAsync(getProjectRequest);

                ProjectItems.Clear();
                var index = (CurrentPage - 1) * RecordsPerPage + 1;
                foreach (var project in pagedResult.Items)
                {
                    ProjectItems.Add(new ProjectItemViewModel(project, index++));
                }

                TotalRecords = pagedResult.TotalCount;
                TotalRecordsText = $"Tổng số bản ghi: {TotalRecords}";
                OnPropertyChanged(nameof(TotalPages));

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
        #endregion
    }
}
