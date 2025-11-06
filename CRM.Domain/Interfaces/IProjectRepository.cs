using CRM.Domain.Filters;
using CRM.Domain.Models;
using CRM.Shared.Results;

namespace CRM.Domain.Interfaces
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<PagedResult<Project>> GetProjectsAsync(ProjectFilter filter);
        Task<IEnumerable<Product>> GetProductsByOpportunityIdAsync(int opportunityId);
    }
}
