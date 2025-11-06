using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class LeadPotentialLevel
{
    public int LeadPotentialLevelId { get; set; }

    public string LeadPotentialLevelCode { get; set; } = null!;

    public string LeadPotentialLevelName { get; set; } = null!;

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
