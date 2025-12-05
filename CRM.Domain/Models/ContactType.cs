namespace CRM.Domain.Models;

public class ContactType
{
    public int ContactTypeId { get; set; }
    public string ContactTypeCode { get; set; } = null!;
    public string ContactTypeName { get; set; } = null!;

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
