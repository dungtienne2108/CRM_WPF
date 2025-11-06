using CommunityToolkit.Mvvm.ComponentModel;
using CRM.Application.Dtos.Deposit;
using CRM.UI.ViewModels.Base;
using System.ComponentModel;

namespace CRM.UI.ViewModels.DepositManagement
{
    public partial class DepositItemViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isSelected;
        [ObservableProperty]
        private DepositDto _depositDto;

        public DepositItemViewModel(DepositDto depositDto, int index)
        {
            DepositDto = depositDto;
            Index = index;
        }

        public int Index { get; set; }

        [DisplayName("ID")]
        public int Id => DepositDto.Id;
        [DisplayName("Mã đặt cọc")]
        public string Code => DepositDto.Code;
        [DisplayName("Tên đặt cọc")]
        public string Name => DepositDto.Name;
        [DisplayName("Cơ hội")]
        public string OpportunityName => DepositDto.OpportunityName;
        [DisplayName("Khách hàng")]
        public string CustomerName => DepositDto.CustomerName;
        [DisplayName("Số tiền")]
        public decimal Amount => DepositDto.Amount;
        [DisplayName("Ngày bắt đầu")]
        public DateTime StartDate => DepositDto.StartDate;
        [DisplayName("Ngày kết thúc")]
        public DateTime EndDate => DepositDto.EndDate;
        [DisplayName("Mô tả")]
        public string? Description => DepositDto.Description;
    }
}
