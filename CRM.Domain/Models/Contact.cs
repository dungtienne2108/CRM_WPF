using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class Contact
{
    public int ContactId { get; set; }

    public string ContactName { get; set; } = null!;

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactAddress { get; set; }

    public int? ContactSalutationId { get; set; }

    public string? ContactDescription { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public virtual ContactSalutation? ContactSalutation { get; set; }

    public virtual ICollection<CustomerContact> CustomerContacts { get; set; } = new List<CustomerContact>();
}
