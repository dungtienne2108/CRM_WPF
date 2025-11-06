using CommunityToolkit.Mvvm.ComponentModel;
using CRM.Application.Dtos.Contract;
using CRM.UI.ViewModels.Base;
using System.ComponentModel;

namespace CRM.UI.ViewModels.ContractManagement
{
    public partial class ContractItemViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isSelected;
        [ObservableProperty]
        private ContractDto _contractDto;

        public ContractItemViewModel(ContractDto contractDto, int index)
        {
            ContractDto = contractDto;
            Index = index;
        }

        public int Index { get; set; }

        [DisplayName("ID")]
        public int Id => ContractDto.Id;
        [DisplayName("Mã hợp đồng")]
        public string Code => ContractDto.Code;
        [DisplayName("Tên hợp đồng")]
        public string Name => ContractDto.Name;
        [DisplayName("Số hợp đồng")]
        public string Number => ContractDto.Number;
        public int CustomerId => ContractDto.CustomerId;
        [DisplayName("Tên khách hàng")]
        public string CustomerName => ContractDto.CustomerName;
        [DisplayName("Giá trị sau thuế")]
        public decimal AmountAfterTax => ContractDto.AmountAfterTax;
        public int StatusId => ContractDto.StatusId;
        [DisplayName("Trạng thái")]
        public string Status => ContractDto.Status;
        [DisplayName("Ngày bắt đầu")]
        public DateTime StartDate => ContractDto.StartDate;
        [DisplayName("Ngày kết thúc")]
        public DateTime EndDate => ContractDto.EndDate;
        [DisplayName("Thời hạn (tháng)")]
        public int Duration => ContractDto.Duration;
    }
}
