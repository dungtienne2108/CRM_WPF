using CRM.Domain.Filters.Base;

namespace CRM.Domain.Filters
{
    public class DepositFilter : FilterBase
    {
        public bool? IsCreatedContract { get; set; }
    }
}
