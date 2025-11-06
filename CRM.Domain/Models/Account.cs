namespace CRM.Domain.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string? AccountCode { get; set; }

    public string AccountName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int? AccountTypeId { get; set; }

    public string? AccountDescription { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual AccountType? AccountType { get; set; }

    public virtual Employee? Employee { get; set; }
}
