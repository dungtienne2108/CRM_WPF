using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Project;
using CRM.Application.Interfaces.Project;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.Admin.ProjectManagement
{
    public partial class AddProductViewModel : ViewModelBase
    {
        private readonly IProjectService _projectService;

        [ObservableProperty]
        private int _projectId;
        [ObservableProperty]
        private string _projectName = string.Empty;
        [ObservableProperty]
        private string _projectAddress = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [MinLength(3, ErrorMessage = "Tên sản phẩm phải có ít nhất 3 ký tự.")]
        private string _productName = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Số tầng không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số tầng phải lớn hơn 0.")]
        private int _productFloors;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Diện tích không được để trống.")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Diện tích phải lớn hơn 0.")]
        private decimal _productArea;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Giá sản phẩm không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        private decimal _productPrice;
        [ObservableProperty]
        private int _productTypeId;

        [ObservableProperty]
        private ObservableCollection<ProductTypeOption> _productTypeOptions = new();

        public AddProductViewModel(IProjectService projectService)
        {
            _projectService = projectService;

            ErrorsChanged += (s, e) => OnPropertyChanged(nameof(CanSave));
        }

        #region public methods
        public async Task LoadDataAsync(int projectId)
        {
            if (projectId <= 0)
                return;

            ProjectId = projectId;
            await GetProjectId(ProjectId);
            await GetProductTypesAsync();
        }
        #endregion

        #region commands
        public bool CanSave => !HasErrors;
        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            if (HasErrors)
                return;

            await CreateProductAsync();
        }

        [RelayCommand]
        private void Cancel()
        {
            // đóng cửa sổ
            System.Windows.Application.Current.Windows
                .OfType<Window>()
                .SingleOrDefault(w => w.DataContext == this)?
                .Close();
        }
        #endregion

        #region private methods
        private async Task CreateProductAsync()
        {
            try
            {
                var createProductRequest = new CreateProductRequest
                {
                    ProjectId = ProjectId,
                    ProductName = ProductName,
                    Floors = ProductFloors,
                    Area = ProductArea,
                    Price = ProductPrice,
                    TypeId = ProductTypeId,
                    StatusId = 1
                };

                var result = await _projectService.CreateProductAsync(createProductRequest);
                if (result.IsSuccess)
                {
                    MessageBox.Show("Tạo sản phẩm thành công!");

                    // đóng cửa sổ
                    System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
                }
                else
                {
                    MessageBox.Show($"Tạo sản phẩm thất bại. Lỗi: {result.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task GetProductTypesAsync()
        {
            try
            {
                var productTypesResult = await _projectService.GetProductTypesAsync();
                if (productTypesResult.IsSuccess)
                {
                    ProductTypeOptions.Clear();
                    foreach (var type in productTypesResult.Value)
                    {
                        ProductTypeOptions.Add(type);
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions (e.g., log the error)
            }
        }

        private async Task GetProjectId(int projectId)
        {
            try
            {
                var projectResult = await _projectService.GetProjectByIdAsync(projectId);
                if (projectResult.IsSuccess)
                {
                    var project = projectResult.Value;
                    ProjectId = project.ProjectId;
                    ProjectName = project.ProjectName;
                    ProjectAddress = project.ProjectAddress;
                }
            }
            catch (Exception)
            {
                // Handle exceptions (e.g., log the error)
            }
        }
        #endregion
    }
}
