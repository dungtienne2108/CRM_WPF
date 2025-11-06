using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class ContactSalutation
{
    public int ContactSalutationId { get; set; }

    public string ContactSalutationCode { get; set; } = null!;

    public string ContactSalutationName { get; set; } = null!;

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
