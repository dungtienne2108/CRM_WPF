using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Interfaces.Project;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.Admin.ProjectManagement;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.Admin.ProjectManagement
{
    public partial class ProjectDetailViewModel : ViewModelBase
    {
        private readonly IProjectService _projectService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private int _projectId;
        [ObservableProperty]
        private string _projectName = string.Empty;
        [ObservableProperty]
        private string _projectCode = string.Empty;
        [ObservableProperty]
        private string _projectDescription = string.Empty;

        [ObservableProperty]
        private ObservableCollection<ProductItemViewModel> _products = new();
        [ObservableProperty]
        private ProductItemViewModel? _selectedProduct;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeleteVisible))]
        private bool _isEditMode = false;
        public bool IsDeleteVisible => !IsEditMode;

        public ProjectDetailViewModel(IProjectService projectService, IServiceProvider serviceProvider)
        {
            _projectService = projectService;
            _serviceProvider = serviceProvider;
        }

        #region Public Methods
        public async Task LoadDataAsync(int projectId)
        {
            if (projectId <= 0)
                return;

            await GetProjectAsync(projectId);
            await GetProductsAsync(projectId);

        }
        #endregion

        #region Commands
        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }

        [RelayCommand]
        private async Task AddProductAsync()
        {
            var addProductViewModel = new AddProductViewModel(_projectService);
            await addProductViewModel.LoadDataAsync(ProjectId);
            var addProductDialog = new Views.Admin.ProjectManagement.AddProductDialog(addProductViewModel);
            addProductDialog.ShowDialog();
            await GetProductsAsync(ProjectId);

        }

        [RelayCommand]
        private async Task OpenDetailAsync(ProductItemViewModel productItem)
        {
            var productDetailViewModel = _serviceProvider.GetRequiredService<ProductDetailViewModel>();
            await productDetailViewModel.LoadDataAsync(productItem);
            var productDetailView = new ProductDetailView(productDetailViewModel);
            productDetailView.ShowDialog();
            await GetProductsAsync(ProjectId);
        }
        #endregion

        #region Private methods
        private async Task GetProjectAsync(int projectId)
        {
            IsLoading = true;
            try
            {
                var projectResult = await _projectService.GetProjectByIdAsync(projectId);
                if (projectResult.IsSuccess)
                {
                    var project = projectResult.Value;
                    ProjectId = project.ProjectId;
                    ProjectName = project.ProjectName;
                    ProjectCode = project.ProjectCode;
                    ProjectDescription = project.ProjectDescription;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task GetProductsAsync(int projectId)
        {
            IsLoading = true;
            try
            {
                var products = await _projectService.GetProductsByProjectIdAsync(projectId);

                if (!products.Any())
                {
                    Products.Clear();
                    MessageBox.Show("Dự án hiện tại đã hết sản phẩm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Products.Clear();

                foreach (var item in products)
                {
                    Products.Add(new ProductItemViewModel(item));
                }
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
