using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Database.Repositories
{
    public class OpportunityRepository : Repository<Opportunity>, IOpportunityRepository
    {
        public OpportunityRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Opportunity>> GetAllOpportunitiesAsync(OpportunityFilter filter)
        {
            var query = _context.Opportunities.AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OpportunityStage)
                .Include(o => o.OpportunityItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Project)
                .AsSplitQuery()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var searchTerm = filter.Keyword.ToLower();
                query = query.Where(opportunity =>
                    opportunity.OpportunityName.ToLower().Contains(searchTerm));
            }

            if (filter.OpportunityStageId.HasValue)
            {
                query = query.Where(o => o.OpportunityStageId == filter.OpportunityStageId.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.CreateDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Opportunity>(items, totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<IEnumerable<Opportunity>> GetOpportunitiesByCustomerIdAsync(int customerId)
        {
            return await _context.Opportunities
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OpportunityStage)
                .Include(o => o.OpportunityItems)
                .AsSplitQuery()
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();
        }

        public Task<Opportunity?> GetOpportunityByIdAsync(int id)
        {
            var query = _context.Opportunities
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OpportunityStage)
                .Include(o => o.OpportunityItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Project)
                .AsSplitQuery()
                .AsQueryable();

            var opportunity = query.FirstOrDefaultAsync(o => o.OpportunityId == id);

            if (opportunity != null)
            {
                // reload entity to ensure all navigation properties are loaded
                _context.Entry(opportunity).Reload();
                return opportunity;
            }

            return null;
        }
    }
}
