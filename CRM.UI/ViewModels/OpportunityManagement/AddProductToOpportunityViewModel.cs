using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Opportunity;
using CRM.Application.Dtos.Project;
using CRM.Application.Interfaces.Opportunity;
using CRM.Application.Interfaces.Project;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.OpportunityManagement
{
    public partial class AddProductToOpportunityViewModel : ViewModelBase
    {
        private readonly IProjectService _projectService;
        private readonly IOpportunityService _opportunityService;

        [ObservableProperty]
        private int _opportunityId;

        [ObservableProperty]
        private int _projectId;
        [ObservableProperty]
        private string _projectAddress = string.Empty;
        [ObservableProperty]
        private decimal _productPrice;
        [ObservableProperty]
        private int _productFloors;
        [ObservableProperty]
        private decimal _productArea;

        [ObservableProperty]
        private ObservableCollection<ProjectDto> _projectOptions = new();
        [ObservableProperty]
        private ObservableCollection<ProductDto> _productOptions = new();
        [ObservableProperty]
        private ProductDto? _selectedProduct;

        public AddProductToOpportunityViewModel(IProjectService projectService,
            IOpportunityService opportunityService)
        {
            _projectService = projectService;
            _opportunityService = opportunityService;
        }

        public async Task LoadDataAsync(int opportunityId)
        {
            OpportunityId = opportunityId;
            await LoadProjectsAsync();
        }

        #region COmmands
        [RelayCommand]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            await AddProductToOpportunityAsync();
        }

        [RelayCommand]
        private void Cancel()
        {
            System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
        }
        #endregion

        #region Private Methods
        private async Task AddProductToOpportunityAsync()
        {
            try
            {
                var addOpportunityItemRequest = new AddOpportunityItemRequest
                {
                    OpportunityId = OpportunityId,
                    ProductId = SelectedProduct.ProductId,
                    Price = SelectedProduct.ProductPrice.HasValue ? SelectedProduct.ProductPrice.Value : 0
                };

                var res = await _opportunityService.AddItemToOpportunityAsync(addOpportunityItemRequest);

                if (res.IsSuccess)
                {
                    MessageBox.Show("Thêm sản phẩm cho cơ hội thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
                }
                else
                {
                    MessageBox.Show($"Lỗi: {res.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch { }
        }

        private async Task LoadProjectsAsync()
        {
            var request = new GetProjectRequest
            {
                PageNumber = 1,
                PageSize = 1000
            };
            var projects = await _projectService.GetProjectsAsync(request);
            ProjectOptions.Clear();

            foreach (var item in projects.Items)
            {
                ProjectOptions.Add(item);
            }
        }

        private async Task LoadProductsAsync(int projectId)
        {
            var products = await _projectService.GetUnsoldProductsByProjectIdAsync(projectId);

            if (!products.Any())
            {
                ProductOptions.Clear();
                MessageBox.Show("Dự án hiện tại đã hết sản phẩm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ProductOptions.Clear();

            foreach (var item in products)
            {
                ProductOptions.Add(item);
            }
        }
        #endregion

        #region Property changed
        partial void OnProjectIdChanged(int value)
        {
            _ = LoadProductsAsync(value);
        }

        partial void OnSelectedProductChanged(ProductDto? value)
        {
            if (value != null)
            {
                ProductArea = value.ProductArea ?? 0;
                ProductFloors = value.ProductFloors;
                ProductPrice = value.ProductPrice ?? 0;
                ProjectAddress = value.ProductAddress ?? string.Empty;
            }
            else
            {
                ProductArea = 0;
                ProductFloors = 0;
                ProductPrice = 0;
                ProjectAddress = string.Empty;
            }
        }
        #endregion
    }
}
