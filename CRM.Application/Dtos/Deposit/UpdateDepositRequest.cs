namespace CRM.Application.Dtos.Deposit
{
    public class UpdateDepositRequest
    {
        public int DepositId { get; set; }
        public string DepositName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public int? ContactId { get; set; }
    }
}
