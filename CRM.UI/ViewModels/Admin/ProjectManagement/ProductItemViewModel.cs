using CommunityToolkit.Mvvm.ComponentModel;
using CRM.Application.Dtos.Project;
using CRM.UI.ViewModels.Base;

namespace CRM.UI.ViewModels.Admin.ProjectManagement
{
    public partial class ProductItemViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isSelected;
        [ObservableProperty]
        private ProductDto _productDto;

        #region Constructor
        public ProductItemViewModel(ProductDto productDto)
        {
            ProductDto = productDto;
        }
        #endregion

        public int Id => ProductDto.ProductId;
        public string Code => ProductDto.ProductCode;
        public string Name => ProductDto.ProductName;
        public int? Floors => ProductDto.ProductFloors;
        public decimal? Area => ProductDto.ProductArea;
        public decimal? Price => ProductDto.ProductPrice;
        public string? Address => ProductDto.ProductAddress;
        public int ProductTypeId => ProductDto.ProductTypeId;
        public string ProductTypeName => ProductDto.ProductTypeName;
        public int ProductStatusId => ProductDto.ProductStatusId;
        public string ProductStatusName => ProductDto.ProductStatusName;
    }
}
