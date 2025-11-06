using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class LeadStage
{
    public int LeadStageId { get; set; }

    public string LeadStageCode { get; set; } = null!;

    public string LeadStageName { get; set; } = null!;

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
