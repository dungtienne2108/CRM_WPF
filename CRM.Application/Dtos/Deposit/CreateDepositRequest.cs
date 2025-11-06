namespace CRM.Application.Dtos.Deposit
{
    public class CreateDepositRequest
    {
        public int OpportunityId { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public int? ContactId { get; set; }
        public int ProductId { get; set; }

        public string DepositName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DepositCosts { get; set; }
        public string? Description { get; set; }
    }
}
