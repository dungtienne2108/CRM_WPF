namespace CRM.Domain.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductCode { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ProductAddress { get; set; }

    public int? ProductNumber { get; set; }

    public int? ProductFloors { get; set; }

    public decimal? ProductArea { get; set; }

    public decimal? ProductPrice { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int ProductTypeId { get; set; }

    public int? ProjectId { get; set; }

    public int ProductStatusId { get; set; }

    public virtual ICollection<ContractItem> ContractItems { get; set; } = new List<ContractItem>();

    public virtual ICollection<LeadItem> LeadItems { get; set; } = new List<LeadItem>();

    public virtual ICollection<OpportunityItem> OpportunityItems { get; set; } = new List<OpportunityItem>();

    public virtual ProductStatus ProductStatus { get; set; } = null!;

    public virtual ProductType ProductType { get; set; } = null!;

    public virtual Project? Project { get; set; }
}
