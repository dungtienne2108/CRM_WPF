namespace CRM.Domain.Models;

public partial class Opportunity
{
    public int OpportunityId { get; set; }

    public string? OpportunityCode { get; set; }

    public string OpportunityName { get; set; } = null!;

    public string? OpportunityDescription { get; set; }

    public DateOnly EndDate { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public int? DaysRemaining { get; set; }

    public int CustomerId { get; set; }

    public int OpportunityStageId { get; set; }

    public int EmployeeId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<OpportunityItem> OpportunityItems { get; set; } = new List<OpportunityItem>();

    public virtual OpportunityStage OpportunityStage { get; set; } = null!;

    public virtual ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();
}
