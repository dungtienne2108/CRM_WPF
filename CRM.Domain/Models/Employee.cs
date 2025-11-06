namespace CRM.Domain.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public string EmployeeIdentityCard { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public DateOnly? EmployeeBirthDay { get; set; }

    public string? EmployeePhone { get; set; }

    public string? EmployeeEmail { get; set; }

    public string? EmployeeAddress { get; set; }

    public string? EmployeeDescription { get; set; }

    public int? GenderId { get; set; }

    public int? EmployeeLevelId { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual EmployeeLevel? EmployeeLevel { get; set; }

    public virtual Gender? Gender { get; set; }

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();

    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
}
