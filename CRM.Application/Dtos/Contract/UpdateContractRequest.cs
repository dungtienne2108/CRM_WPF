namespace CRM.Application.Dtos.Contract
{
    public class UpdateContractRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public int ContractTypeId { get; set; }
        public int ContractStageId { get; set; }
        public decimal AmountAfterTax { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal AmountBeforeTax { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; } = string.Empty;
    }
}
