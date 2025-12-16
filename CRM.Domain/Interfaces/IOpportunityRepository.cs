using CRM.Domain.Filters;
using CRM.Domain.Models;
using CRM.Shared.Results;

namespace CRM.Domain.Interfaces
{
    public interface IOpportunityRepository : IRepository<Opportunity>
    {
        Task<Opportunity?> GetOpportunityByIdAsync(int id);
        Task<IEnumerable<Opportunity>> GetOpportunitiesByCustomerIdAsync(int customerId);
        Task<PagedResult<Opportunity>> GetAllOpportunitiesAsync(OpportunityFilter filter);
        Task UpdateOpportunityStageAsync(int opportunityId, int newStage);
    }
}
