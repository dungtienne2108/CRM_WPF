using System.ComponentModel;

namespace CRM.Application.Dtos.Deposit
{
    public class DepositDto
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Mã đặt cọc")]
        public required string Code { get; set; }

        [DisplayName("Tên đặt cọc")]
        public required string Name { get; set; }

        public int OpportunityId { get; set; }

        [DisplayName("Cơ hội kinh doanh")]
        public required string OpportunityName { get; set; }

        public int CustomerId { get; set; }

        [DisplayName("Khách hàng")]
        public required string CustomerName { get; set; }

        [DisplayName("Số tiền đặt cọc")]
        public decimal Amount { get; set; }

        [DisplayName("Ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        [DisplayName("Ngày kết thúc")]
        public DateTime EndDate { get; set; }

        [DisplayName("Mô tả")]
        public string? Description { get; set; }

        public int EmployeeId { get; set; }

        [DisplayName("Nhân viên phụ trách")]
        public required string EmployeeName { get; set; }

        public int? ContactId { get; set; }

        public int ProjectId { get; set; }

        [DisplayName("Dự án")]
        public required string ProjectName { get; set; }

        public int ProductId { get; set; }

        [DisplayName("Sản phẩm")]
        public required string ProductName { get; set; }

        [DisplayName("Giá sản phẩm")]
        public decimal ProductPrice { get; set; }
    }
}
