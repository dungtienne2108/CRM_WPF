namespace CRM.Domain.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public string? PaymentCode { get; set; }

    public int InvoiceId { get; set; }

    public int CustomerId { get; set; }

    public int PaymentMethodId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal RemainAmount { get; set; }

    public string? Description { get; set; }

    public string? CreateBy { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}
