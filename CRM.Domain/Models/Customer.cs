namespace CRM.Domain.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? CustomerCode { get; set; }

    public string CustomerName { get; set; } = null!;

    public string? CustomerIdentityCard { get; set; }

    public string? CustomerPhone { get; set; }

    public string? CustomerEmail { get; set; }

    public string? CustomerAddress { get; set; }

    public DateOnly? CustomerBirthDay { get; set; }

    public string? CustomerDescription { get; set; }

    public int? GenderId { get; set; }

    public int? EmployeeId { get; set; }
    public int? CustomerTypeId { get; set; }

    public int? LeadId { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<CustomerContact> CustomerContacts { get; set; } = new List<CustomerContact>();

    public virtual ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();

    public virtual Employee? Employee { get; set; }

    public virtual Gender? Gender { get; set; }

    public virtual CustomerType? CustomerType { get; set; }

    public virtual Lead? Lead { get; set; }

    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
}
