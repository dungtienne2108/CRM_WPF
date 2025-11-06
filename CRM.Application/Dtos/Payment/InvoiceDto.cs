using System.ComponentModel;

namespace CRM.Application.Dtos.Payment
{
    public class InvoiceDto
    {
        public int Id { get; set; }

        [DisplayName("Mã hóa đơn")]
        public string Code { get; set; } = string.Empty;

        [DisplayName("Số hóa đơn")]
        public string Number { get; set; } = string.Empty;

        public int ContractId { get; set; }

        [DisplayName("Hợp đồng liên quan")]
        public string ContractName { get; set; } = string.Empty;

        public int InstallmentScheduleId { get; set; }

        [DisplayName("Kỳ thanh toán")]
        public string InstallmentScheduleName { get; set; } = string.Empty;

        [DisplayName("Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = string.Empty;

        [DisplayName("Số tiền thanh toán")]
        public decimal Amount { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Hạn thanh toán")]
        public DateTime DueDate { get; set; }

        [DisplayName("Trạng thái")]
        public string Status { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        [DisplayName("Khách hàng")]
        public string CustomerName { get; set; } = string.Empty;
    }
}
