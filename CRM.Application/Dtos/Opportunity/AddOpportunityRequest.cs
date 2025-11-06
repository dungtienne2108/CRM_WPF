namespace CRM.Application.Dtos.Opportunity
{
    public class AddOpportunityRequest
    {
        public string OpportunityName { get; set; } = string.Empty;
        public string? OpportunityDescription { get; set; }
        public int OpportunityStatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public List<AddOpportunityItemRequest> OpportunityItems { get; set; } = new();
    }

    public class AddOpportunityItemRequest
    {
        public int OpportunityId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal ExpectedPrice { get; set; }
    }
}
