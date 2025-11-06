namespace CRM.Application.Dtos.Payment
{
    public class CreatePaymentScheduleRequest
    {
        public int ContractId { get; set; }
        public string InstallmentName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal ContractValuePercentage { get; set; }
        public DateTime DueDate { get; set; }
    }
}
