using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class LeadItem
{
    public int LeadProductId { get; set; }

    public int? LeadId { get; set; }

    public int? ProductId { get; set; }

    public virtual Lead? Lead { get; set; }

    public virtual Product? Product { get; set; }
}
