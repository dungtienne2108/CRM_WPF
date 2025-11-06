using CommunityToolkit.Mvvm.ComponentModel;
using CRM.Application.Dtos.Contact;
using CRM.UI.ViewModels.Base;
using System.ComponentModel;

namespace CRM.UI.ViewModels.ContactManagement
{
    public partial class ContactItemViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isSelected;
        [ObservableProperty]
        private ContactDto _contactDto;

        public ContactItemViewModel(ContactDto contactDto, int index)
        {
            ContactDto = contactDto;
            Index = index;
        }

        public int Index { get; set; }

        [DisplayName("ID")]
        public int Id => ContactDto.Id;
        [DisplayName("Tên khách hàng")]
        public string Name => ContactDto.Name;
        [DisplayName("Số điện thoại")]
        public string Phone => ContactDto.Phone;
        [DisplayName("Email")]
        public string Email => ContactDto.Email;
        [DisplayName("Địa chỉ")]
        public string Address => ContactDto.Address;
        public int SalutationId => ContactDto.SalutationId;
        [DisplayName("Xưng hô")]
        public string Salutation => ContactDto.Salutation;
        [DisplayName("Ngày tạo")]
        public DateTime CreatedDate => ContactDto.CreatedDate;
    }
}
