using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string PaymentMethodCode { get; set; } = null!;

    public string PaymentMethodName { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
