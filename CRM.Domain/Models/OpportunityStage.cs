using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class OpportunityStage
{
    public int OpportunityStageId { get; set; }

    public string OpportunityStageCode { get; set; } = null!;

    public string OpportunityStageName { get; set; } = null!;

    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
}
