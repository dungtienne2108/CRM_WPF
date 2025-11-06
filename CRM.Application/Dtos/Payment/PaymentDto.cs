using System.ComponentModel;

namespace CRM.Application.Dtos.Payment
{
    public class PaymentDto
    {
        public int Id { get; set; }

        [DisplayName("Mã phiếu thanh toán")]
        public string Code { get; set; } = string.Empty;

        [DisplayName("Số phiếu thanh toán")]
        public string Number { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        [DisplayName("Khách hàng")]
        public string CustomerName { get; set; } = string.Empty;

        public int InvoiceId { get; set; }

        [DisplayName("Hóa đơn liên quan")]
        public string InvoiceName { get; set; } = string.Empty;

        [DisplayName("Số tiền thanh toán")]
        public decimal Amount { get; set; }

        [DisplayName("Ngày thanh toán")]
        public DateTime PaymentDate { get; set; }

        [DisplayName("Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = string.Empty;

        public int PaymentMethodId { get; set; }

        [DisplayName("Mô tả")]
        public string Description { get; set; } = string.Empty;
    }
}
