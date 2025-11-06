namespace CRM.Application.Dtos.Payment
{
    public class PaymentScheduleDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string ContractName { get; set; } = string.Empty;
        public string InstallmentName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal ContractValuePercentage { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Status { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
    }
}
