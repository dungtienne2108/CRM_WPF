using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class ContractItem
{
    public int ContractItemId { get; set; }

    public int? ContractId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? SalePrice { get; set; }

    public decimal? CostTax { get; set; }

    public decimal? GrandTotal { get; set; }

    public virtual Contract? Contract { get; set; }

    public virtual Product? Product { get; set; }
}
