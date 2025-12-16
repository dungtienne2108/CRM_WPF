using CRM.Domain.Filters;
using CRM.Domain.Models;
using CRM.Shared.Results;

namespace CRM.Domain.Interfaces
{
    public interface ILeadRepository : IRepository<Lead>
    {
        Task<Lead?> GetLeadByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResult<Lead>> GetLeadsAsync(LeadFilter filter, CancellationToken cancellationToken = default);
        Task UpdateLeadStageAsync(int leadId, int newStageId, CancellationToken cancellationToken = default);
    }
}
