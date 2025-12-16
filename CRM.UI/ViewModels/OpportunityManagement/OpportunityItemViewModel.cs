using CommunityToolkit.Mvvm.ComponentModel;
using CRM.Application.Dtos.Customer;
using CRM.Application.Dtos.Employee;
using CRM.Application.Dtos.Lead;
using CRM.Application.Dtos.Opportunity;
using CRM.Application.Dtos.Project;
using CRM.UI.ViewModels.Base;
using System.ComponentModel;

namespace CRM.UI.ViewModels.OpportunityManagement
{
    public partial class OpportunityItemViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isSelected;
        [ObservableProperty]
        private OpportunityDto _opportunityDto;

        public OpportunityItemViewModel(int index, OpportunityDto opportunityDto)
        {
            Index = index;
            OpportunityDto = opportunityDto;
        }

        public int Index { get; set; }

        [DisplayName("ID")]
        public int OpportunityId => OpportunityDto.OpportunityId;
        [DisplayName("Mã Cơ Hội")]
        public string? OpportunityCode => OpportunityDto.OpportunityCode;
        [DisplayName("Tên Cơ Hội")]
        public string OpportunityName => OpportunityDto.OpportunityName;
        [DisplayName("Mô Tả Cơ Hội")]
        public string? OpportunityDescription => OpportunityDto.OpportunityDescription;
        [DisplayName("Ngày kết thúc")]
        public DateOnly EndDate => OpportunityDto.EndDate;
        [DisplayName("Ngày tạo")]
        public DateTime? CreateDate => OpportunityDto.CreateDate;
        [DisplayName("Giá dự kiến")]
        public decimal ExpectedPrice => OpportunityDto.ExpectedPrice;
        [DisplayName("Giá thực tế")]
        public decimal RealPrice => OpportunityDto.RealPrice;
        public bool IsDeposited => OpportunityDto.IsDeposited;

        public List<ProductDto> Products => OpportunityDto.Products;
        public CustomerDto Customer => OpportunityDto.Customer;
        public OpportunityStatusOption OpportunityStatus => OpportunityDto.OpportunityStatus;
        public EmployeeDto Employee => OpportunityDto.Employee;

    }
}
