using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Project;
using CRM.Application.Interfaces.Project;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.Admin.ProjectManagement
{
    public partial class ProductDetailViewModel : ViewModelBase
    {
        private readonly IProjectService _projectService;

        [ObservableProperty]
        private int _productId;
        [ObservableProperty]
        private string _productName = string.Empty;
        [ObservableProperty]
        private string _productCode = string.Empty;
        [ObservableProperty]
        private decimal _productArea;
        [ObservableProperty]
        private decimal _productPrice;
        [ObservableProperty]
        private int _productFloors;
        [ObservableProperty]
        private int _productTypeId;
        [ObservableProperty]
        private string _productTypeName = string.Empty;
        [ObservableProperty]
        private int _productStatusId;
        [ObservableProperty]
        private string _productStatusName = string.Empty;

        [ObservableProperty]
        private ObservableCollection<ProductTypeOption> _productTypeOptions = new();
        [ObservableProperty]
        private ObservableCollection<ProductStatusOption> _productStatusOptions = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeleteVisible))]
        private bool _isEditMode = false;
        public bool IsDeleteVisible => !IsEditMode;

        public ProductDetailViewModel(IProjectService projectService)
        {
            _projectService = projectService;

            ErrorsChanged += (s, e) => OnPropertyChanged(nameof(CanSave));
        }

        #region Public methods
        public async Task LoadDataAsync(ProductItemViewModel productItem)
        {
            if (productItem == null)
            {
                return;
            }

            ProductId = productItem.Id;
            await GetProductAsync(productItem.Id);
            await GetProductStatusesAsync();
            await GetProducTypesAsync();
        }
        #endregion

        #region Commands
        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }

        public bool CanSave => !HasErrors;

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            if (HasErrors)
                return;

            await UpdateProductAsync();
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (ProductId <= 0)
            {
                MessageBox.Show("Thông tin sản phẩm không hợp lệ", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                var res = await _projectService.DeleteProductAsync(ProductId);

                if (res.IsSuccess)
                {
                    MessageBox.Show("Xóa sản phẩm thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
                }
                else
                {
                    MessageBox.Show("Xóa sản phẩm thất bại", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch { }
        }
        #endregion

        #region Private methods
        private async Task UpdateProductAsync()
        {
            try
            {
                var updateProductRequest = new UpdateProductRequest
                {
                    ProductTypeId = ProductTypeId,
                    ProductArea = ProductArea,
                    ProductFloors = ProductFloors,
                    ProductId = ProductId,
                    ProductName = ProductName,
                    ProductPrice = ProductPrice,
                    ProductStatusId = ProductStatusId
                };

                var res = await _projectService.UpdateProductAsync(updateProductRequest);

                if (res.IsSuccess)
                {
                    MessageBox.Show("Cập nhật thông tin sản phẩm thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetProductAsync(res.Value.ProductId);
                    return;
                }
                else
                {
                    MessageBox.Show("Lỗi xảy ra khi cập nhật sản phẩm", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch { }
        }

        private async Task GetProductAsync(int productId)
        {
            if (productId <= 0)
            {
                MessageBox.Show("Không tìm thấy sản phẩm", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var productRes = await _projectService.GetProductByIdAsync(productId);

                if (productRes.IsSuccess)
                {
                    var product = productRes.Value;
                    ProductName = product.ProductName;
                    ProductCode = product.ProductCode;
                    ProductArea = product.ProductArea.HasValue ? product.ProductArea.Value : 0;
                    ProductPrice = product.ProductPrice.HasValue ? product.ProductPrice.Value : 0;
                    ProductFloors = product.ProductFloors;
                    ProductTypeId = product.ProductTypeId;
                    ProductTypeName = product.ProductTypeName;
                    ProductStatusId = product.ProductStatusId;
                    ProductStatusName = product.ProductStatusName;
                }
                else
                {
                    MessageBox.Show("Lỗi xảy ra khi lấy thông tin sản phẩm", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch { }
        }

        private async Task GetProducTypesAsync()
        {
            try
            {
                var productTypesResult = await _projectService.GetProductTypesAsync();
                if (productTypesResult.IsSuccess)
                {
                    var productTypes = productTypesResult.Value;

                    ProductTypeOptions.Clear();

                    foreach (var item in productTypes)
                    {
                        ProductTypeOptions.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show("Lỗi khi lấy loại sản phẩm", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async Task GetProductStatusesAsync()
        {
            try
            {
                var productStatusesResult = await _projectService.GetProductStatusesAsync();
                if (productStatusesResult.IsSuccess)
                {
                    var productStatuses = productStatusesResult.Value;

                    ProductStatusOptions.Clear();

                    foreach (var item in productStatuses)
                    {
                        ProductStatusOptions.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show("Lỗi khi lấy trạng thái sản phẩm", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch { }
        }

        #endregion

    }
}
