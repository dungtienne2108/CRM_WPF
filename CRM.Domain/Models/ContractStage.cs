using System;
using System.Collections.Generic;

namespace CRM.Domain.Models;

public partial class ContractStage
{
    public int ContractStageId { get; set; }

    public string ContractStageCode { get; set; } = null!;

    public string ContractStageName { get; set; } = null!;

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
