namespace CRM.Application.Dtos.Contract
{
    public class CreateContractRequest
    {
        public string ContractNumber { get; set; } = string.Empty;
        public string ContractName { get; set; } = string.Empty;
        public string Seller { get; set; } = "Công ty FLC";
        public decimal AmountBeforeTax { get; set; }
        public decimal Tax { get; set; }
        public decimal AmountAfterTax { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public int ContractTypeId { get; set; }
        public int ContractStageId { get; set; }
        public int DepositId { get; set; }
        public int ProductId { get; set; }
    }
}
