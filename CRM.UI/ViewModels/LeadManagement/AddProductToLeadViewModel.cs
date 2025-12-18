using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Project;
using CRM.Application.Interfaces.Leads;
using CRM.Application.Interfaces.Project;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.LeadManagement
{
    public partial class AddProductToLeadViewModel : ViewModelBase
    {
        private readonly IProjectService _projectService;
        private readonly ILeadService _leadService;

        [ObservableProperty]
        private int _leadId;

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

        public AddProductToLeadViewModel(IProjectService projectService,
            ILeadService leadService)
        {
            _projectService = projectService;
            _leadService = leadService;
        }

        public async Task LoadDataAsync(int leadId)
        {
            LeadId = leadId;
            await LoadProjectsAsync();
        }

        #region COmmands
        [RelayCommand]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            await AddProductToLeadAsync();
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
        private async Task AddProductToLeadAsync()
        {
            try
            {
                if (SelectedProduct == null)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var res = await _leadService.AddItemToLeadAsync(LeadId, SelectedProduct.ProductId);

                if (res.IsSuccess)
                {
                    MessageBox.Show("Thêm sản phẩm cho khách hàng tiềm năng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

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
