using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class OpportunityItem
{
    public int OpportunityItemId { get; set; }

    public int? OpportunityId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? SalePrice { get; set; }

    public decimal? ExceptedProfit { get; set; }

    public virtual Opportunity? Opportunity { get; set; }

    public virtual Product? Product { get; set; }
}
