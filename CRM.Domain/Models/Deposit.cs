namespace CRM.Domain.Models;

public partial class Deposit
{
    public int DepositId { get; set; }

    public string? DepositCode { get; set; }

    public string DepositName { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? CreateBy { get; set; }

    public bool IsCreatedContract { get; set; }

    public bool IsDeleted { get; set; }

    public int? CustomerId { get; set; }

    public int? OpportunityId { get; set; }

    public int? ContactId { get; set; }

    public int? EmployeeId { get; set; }

    public decimal? DepositCost { get; set; }

    public string? Description { get; set; }

    public int ProductId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Opportunity? Opportunity { get; set; }

    public virtual Contact? Contact { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Product Product { get; set; } = null!;
}
