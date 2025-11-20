namespace CRM.Application.Dtos.Payment
{
    public class CreatePaymentRequest
    {
        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Description { get; set; }
        public decimal RemainAmount { get; set; }
    }
}
