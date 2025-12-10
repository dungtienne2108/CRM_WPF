using CRM.Application.Dtos.Customer;
using CRM.Application.Dtos.Employee;
using CRM.Application.Dtos.Lead;
using CRM.Application.Dtos.Project;
using System.ComponentModel;

namespace CRM.Application.Dtos.Opportunity
{
    public class OpportunityDto
    {
        [DisplayName("ID")]
        public int OpportunityId { get; set; }

        [DisplayName("Mã cơ hội")]
        public string? OpportunityCode { get; set; }

        [DisplayName("Tên cơ hội")]
        public string OpportunityName { get; set; } = null!;

        [DisplayName("Mô tả cơ hội")]
        public string? OpportunityDescription { get; set; }

        [DisplayName("Ngày kết thúc")]
        public DateOnly EndDate { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime? CreateDate { get; set; }

        [DisplayName("Giá trị kỳ vọng")]
        public decimal ExpectedPrice { get; set; }

        [DisplayName("Giá trị thực tế")]
        public decimal RealPrice { get; set; }

        public List<ProductDto> Products { get; set; } = new();

        [DisplayName("Khách hàng")]
        public string CustomerName => Customer.Name;

        public CustomerDto Customer { get; set; } = null!;

        [DisplayName("Trạng thái")]
        public string Status => OpportunityStatus.Name;

        public OpportunityStatusOption OpportunityStatus { get; set; } = null!;

        [DisplayName("Nhân viên phụ trách")]
        public string EmployeeName => Employee.Name;

        public EmployeeDto Employee { get; set; } = null!;

        public List<OpportunityItemDto> OpportunityItems { get; set; } = new();

        [DisplayName("Số lượng sản phẩm")]
        public int ItemCount => OpportunityItems.Count;
    }

    public class OpportunityItemDto
    {
        public int OpportunityId { get; set; }
        public int ProductId { get; set; }

        [DisplayName("Tên sản phẩm")]
        public string ProductName { get; set; } = null!;

        public string ProjectName { get; set; } = null!;

        public int ProjectId { get; set; }

        [DisplayName("Số lượng")]
        public int Quantity { get; set; }

        [DisplayName("Đơn giá")]
        public decimal Price { get; set; }

        public int ProductStatusId { get; set; }

        [DisplayName("Trạng thái sản phẩm")]
        public string ProductStatus { get; set; } = null!;
    }
}
