using System.ComponentModel;

namespace CRM.Application.Dtos.Contract
{
    public class ContractDto
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Mã hợp đồng")]
        public string Code { get; set; } = string.Empty;

        [DisplayName("Tên hợp đồng")]
        public string Name { get; set; } = string.Empty;

        [DisplayName("Số hợp đồng")]
        public string Number { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        [DisplayName("Khách hàng")]
        public string CustomerName { get; set; } = string.Empty;

        [DisplayName("Thành tiền sau thuế")]
        public decimal AmountAfterTax { get; set; }

        [DisplayName("Thuế")]
        public decimal Tax { get; set; }

        [DisplayName("Thành tiền trước thuế")]
        public decimal AmountBeforeTax { get; set; }

        [DisplayName("Tổng giá trị hợp đồng")]
        public decimal Amount { get; set; }

        public int TypeId { get; set; }

        [DisplayName("Loại hợp đồng")]
        public required string Type { get; set; }

        public int StatusId { get; set; }

        [DisplayName("Trạng thái")]
        public string Status { get; set; } = string.Empty;

        [DisplayName("Ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        [DisplayName("Ngày kết thúc")]
        public DateTime EndDate { get; set; }

        [DisplayName("Thời hạn (ngày)")]
        public int Duration => (EndDate - StartDate).Days;

        public int ProductId { get; set; }

        [DisplayName("Sản phẩm")]
        public string ProductName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        [DisplayName("Dự án")]
        public string ProjectName { get; set; } = string.Empty;

        public int EmployeeId { get; set; }

        [DisplayName("Nhân viên phụ trách")]
        public string EmployeeName { get; set; } = string.Empty;

        public int DepositId { get; set; }

        [DisplayName("Mô tả")]
        public string? Description { get; set; }

        [DisplayName("Số tiền đã thanh toán")]
        public decimal PaidAmount { get; set; }

        [DisplayName("Số tiền còn lại")]
        public decimal RemainingAmount { get; set; }

        public List<ContractDocumentDto> Documents { get; set; } = new List<ContractDocumentDto>();
    }
}
