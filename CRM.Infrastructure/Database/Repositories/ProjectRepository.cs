using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Database.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsByOpportunityIdAsync(int opportunityId)
        {
            var products = await _context.Products
                .Where(p => p.OpportunityItems.Any(oi => oi.OpportunityId == opportunityId))
                .ToListAsync();

            return products;
        }

        public async Task<PagedResult<Project>> GetProjectsAsync(ProjectFilter filter)
        {
            var query = _context.Projects
                .Include(p => p.Products)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(p => p.ProjectName.Contains(filter.Keyword) ||
                                         (p.ProjectCode != null && p.ProjectCode.Contains(filter.Keyword)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new(items, totalCount, filter.PageNumber, filter.PageSize);
        }
    }
}
