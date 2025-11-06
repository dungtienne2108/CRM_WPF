using CRM.Application.Dtos.Lead;
using CRM.Application.Dtos.Opportunity;
using CRM.Shared.Results;

namespace CRM.Application.Interfaces.Opportunity
{
    public interface IOpportunityService
    {
        Task<Result> AddItemToOpportunityAsync(AddOpportunityItemRequest request);
        Task<Result<OpportunityDto>> GetOpportunityByIdAsync(int id);
        Task<Result<IEnumerable<OpportunityDto>>> GetOpportunitiesByCustomerIdAsync(int customerId);
        Task<Result<OpportunityDto>> UpdateOpportunityStageAsync(int opportunityId, int newStageId);
        //Task<OpportunityDto> UpdateOpportunityAsync(UpdateOpportunityRequest request);
        Task<List<OpportunityStatusOption>> GetAllOpportunityStatusesAsync();
        Task<OpportunityDto> AddOpportunityAsync(AddOpportunityRequest request);
        Task<PagedResult<OpportunityDto>> GetAllOpportunitiesAsync(GetOpportunityRequest request);
        Task<Result<OpportunityDto>> UpdateOpportunityAsync(UpdateOpportunityRequest request);
        Task<Result> DeleteOpportunityAsync(int opportunityId);
        Task<Result> RemoveProductFromOpportunityAsync(int opportunityId, int productId);
    }
}
