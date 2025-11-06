using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class ContractType
{
    public int ContractTypeId { get; set; }

    public string ContractTypeCode { get; set; } = null!;

    public string ContractTypeName { get; set; } = null!;

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
