using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class EmployeeLevel
{
    public int EmployeeLevelId { get; set; }

    public string EmployeeLevelCode { get; set; } = null!;

    public string EmployeeLevelName { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
