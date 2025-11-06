namespace CRM.Domain.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public string? InvoiceCode { get; set; }

    public int ContractId { get; set; }

    public DateOnly? DueDate { get; set; }

    public decimal TotalAmount { get; set; }

    public int InstallmentScheduleId { get; set; }

    public bool IsPaid { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public virtual Contract Contract { get; set; } = null!;

    public virtual InstallmentSchedule InstallmentSchedule { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
