namespace CRM.Application.Dtos.Opportunity
{
    public class GetOpportunityRequest
    {
        public string? Keyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? OpportunityStageId { get; set; }
    }
}
