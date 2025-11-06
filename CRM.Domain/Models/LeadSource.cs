using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class LeadSource
{
    public int LeadSourceId { get; set; }

    public string LeadSourceCode { get; set; } = null!;

    public string LeadSourceName { get; set; } = null!;

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
