namespace CRM.Domain.Models;

public partial class Contract
{
    public int ContractId { get; set; }

    public string? ContractCode { get; set; }

    public string ContractName { get; set; } = null!;

    public string ContractNumber { get; set; } = null!;

    public string? Seller { get; set; }

    public decimal AmountAfterTax { get; set; }

    public decimal Tax { get; set; }

    public decimal AmountBeforeTax { get; set; }

    public decimal Amount { get; set; }

    public string? ContractDescription { get; set; }

    public DateOnly ContractStartDate { get; set; }

    public DateOnly ContractEndDate { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public int? DaysRemaining { get; set; }

    public int CustomerId { get; set; }

    public int ContractTypeId { get; set; }

    public int ContractStageId { get; set; }

    public int EmployeeId { get; set; }

    public int DepositId { get; set; }

    public int ProductId { get; set; }

    public virtual ICollection<ContractItem> ContractItems { get; set; } = new List<ContractItem>();

    public virtual ContractStage ContractStage { get; set; } = null!;

    public virtual ContractType ContractType { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual Deposit Deposit { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<InstallmentSchedule> InstallmentSchedules { get; set; } = new List<InstallmentSchedule>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<ContractDocument> ContractDocuments { get; set; } = new List<ContractDocument>();

}
