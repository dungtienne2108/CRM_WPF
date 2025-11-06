namespace CRM.Application.Dtos.Payment
{
    public class CreateInvoiceRequest
    {
        public int ContractId { get; set; }
        public int PaymentScheduleId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
    }
}
