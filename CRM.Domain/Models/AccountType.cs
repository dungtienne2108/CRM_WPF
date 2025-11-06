namespace CRM.Domain.Models;

public partial class AccountType
{
    public int AccountTypeId { get; set; }

    public string AccountTypeCode { get; set; } = null!;

    public string AccountTypeName { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
