using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class ProductType
{
    public int ProductTypeId { get; set; }

    public string? ProductTypeCode { get; set; }

    public string? ProductTypeName { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
