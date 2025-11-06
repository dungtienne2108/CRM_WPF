namespace CRM.Domain.Models;

public partial class Lead
{
    public int LeadId { get; set; }

    public string? LeadCode { get; set; }

    public string LeadName { get; set; } = null!;

    public string? LeadPhone { get; set; }

    public string? LeadEmail { get; set; }

    public string? LeadAddress { get; set; }

    public int? LeadSourceId { get; set; }

    public int? LeadPotentialLevelId { get; set; }

    public int? LeadStageId { get; set; }

    public string? LeadDescription { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? CreateBy { get; set; }

    public int? Daypassed { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<LeadItem> LeadItems { get; set; } = new List<LeadItem>();

    public virtual LeadPotentialLevel? LeadPotentialLevel { get; set; }

    public virtual LeadSource? LeadSource { get; set; }

    public virtual LeadStage? LeadStage { get; set; }
}
