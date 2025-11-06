using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class Gender
{
    public int GenderId { get; set; }

    public string GenderCode { get; set; } = null!;

    public string GenderName { get; set; } = null!;

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
