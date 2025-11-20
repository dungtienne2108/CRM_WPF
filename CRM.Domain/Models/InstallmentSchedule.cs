namespace CRM.Domain.Models;

public partial class InstallmentSchedule
{
    public int InstallmentId { get; set; }

    public int ContractId { get; set; }

    public int? InstallmentNo { get; set; }

    public string InstallmentName { get; set; } = null!;

    public decimal ContractValuePercentage { get; set; }

    public DateOnly? DueDate { get; set; }

    public decimal Amount { get; set; }

    public string? Status { get; set; }

    public string? InvoiceNumber { get; set; }

    public bool IsDeposited { get; set; }

    public virtual Contract Contract { get; set; } = null!;
}
