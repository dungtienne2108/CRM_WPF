namespace CRM.Application.Dtos.Opportunity
{
    public class UpdateOpportunityRequest
    {
        public int OpportunityId { get; set; }
        public string OpportunityName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
    }
}
