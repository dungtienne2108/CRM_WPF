namespace CRM.Domain.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string? ProjectCode { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? ProjectAddress { get; set; }

    public DateOnly? ProjectStartDate { get; set; }

    public DateOnly? ProjectEndDate { get; set; }

    public string? ProjectStatus { get; set; }

    public string? ProjectDescription { get; set; }

    public decimal? TotalArea { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public int? DaysRemaining { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
