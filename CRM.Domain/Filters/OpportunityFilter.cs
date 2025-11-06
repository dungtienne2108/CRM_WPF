using CRM.Domain.Filters.Base;

namespace CRM.Domain.Filters
{
    public class OpportunityFilter : FilterBase
    {
        public int? OpportunityStageId { get; set; }
    }
}
