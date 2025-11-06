using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Project;
using CRM.Application.Interfaces.Project;
using CRM.UI.ViewModels.Base;
using System.Windows;

namespace CRM.UI.ViewModels.Admin.ProjectManagement
{
    public partial class AddProjectViewModel : ViewModelBase
    {
        private readonly IProjectService _projectService;

        [ObservableProperty]
        private string _projectName = string.Empty;
        [ObservableProperty]
        private DateTime _startDate = DateTime.UtcNow;
        [ObservableProperty]
        private DateTime _endDate = DateTime.UtcNow;
        [ObservableProperty]
        private string _projectAddress = string.Empty;
        [ObservableProperty]
        private decimal _projectArea;

        public AddProjectViewModel(IProjectService projectService)
        {
            _projectService = projectService;
        }

        #region Commands
        [RelayCommand]
        private void Cancel()
        {
            System.Windows.Application.Current.Windows
                .OfType<System.Windows.Window>()
                .SingleOrDefault(w => w.DataContext == this)?
                .Close();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();
            if (HasErrors)
                return;

            await CreateProjectAsync();
        }
        #endregion

        #region Private Methods
        private async Task CreateProjectAsync()
        {
            var request = new CreateProjectRequest
            {
                ProjectName = ProjectName,
                StartDate = StartDate,
                EndDate = EndDate,
                ProjectAddress = ProjectAddress,
                ProjectArea = ProjectArea
            };
            var result = await _projectService.CreateProjectAsync(request);
            if (result.IsSuccess)
            {
                MessageBox.Show("Dự án đã được tạo thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                // đóng cửa sổ
                System.Windows.Application.Current.Windows
                    .OfType<Window>()
                    .SingleOrDefault(w => w.DataContext == this)?
                    .Close();
            }
            else
            {
                MessageBox.Show($"Tạo dự án thất bại. Lỗi: {result.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        #endregion
    }
}
