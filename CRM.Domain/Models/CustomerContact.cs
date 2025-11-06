using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class CustomerContact
{
    public int CustomerContactId { get; set; }

    public int CustomerId { get; set; }

    public int ContactId { get; set; }

    public string? Role { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Notes { get; set; }

    public virtual Contact Contact { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
