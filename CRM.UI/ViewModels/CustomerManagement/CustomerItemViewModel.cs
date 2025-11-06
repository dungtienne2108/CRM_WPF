using CommunityToolkit.Mvvm.ComponentModel;
using CRM.Application.Dtos.Customer;
using CRM.UI.ViewModels.Base;
using System.ComponentModel;

namespace CRM.UI.ViewModels.CustomerManagement
{
    public partial class CustomerItemViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isSelected;
        [ObservableProperty]
        private CustomerDto _customerDto;

        public CustomerItemViewModel(CustomerDto customerDto, int index)
        {
            CustomerDto = customerDto;
            Index = index;
        }

        public int Index { get; set; }

        [DisplayName("ID")]
        public int Id => CustomerDto.Id;
        [DisplayName("Mã khách hàng")]
        public string? Code => CustomerDto.Code;
        [DisplayName("Tên khách hàng")]
        public string Name => CustomerDto.Name;
        [DisplayName("Căn cước/CMND")]
        public string? CustomerIdentityCard => CustomerDto.CustomerIdentityCard;
        public int CustomerTypeId => CustomerDto.TypeId;
        [DisplayName("Loại khách hàng")]
        public string CustomerTypeName => CustomerDto.TypeName;
        [DisplayName("Điện thoại")]
        public string? Phone => CustomerDto.Phone;
        [DisplayName("Email")]
        public string? Email => CustomerDto.Email;
        [DisplayName("Địa chỉ")]
        public string? Address => CustomerDto.Address;
        [DisplayName("Ngày sinh")]
        public string? Gender => CustomerDto.Gender;
        [DisplayName("Mô tả")]
        public string? Description => CustomerDto.Description;
    }
}
