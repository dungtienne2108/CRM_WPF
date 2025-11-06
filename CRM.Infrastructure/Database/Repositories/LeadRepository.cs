using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Database.Repositories
{
    public sealed class LeadRepository : Repository<Lead>, ILeadRepository
    {
        public LeadRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Lead?> GetLeadByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var lead = await _context.Leads
                .Include(l => l.Employee)
                .Include(l => l.LeadPotentialLevel)
                .Include(l => l.LeadStage)
                .Include(l => l.LeadSource)
                .FirstOrDefaultAsync(l => l.LeadId == id, cancellationToken);

            if (lead != null)
                await _context.Entry(lead).ReloadAsync();

            return lead;
        }

        public async Task<PagedResult<Lead>> GetLeadsAsync(LeadFilter filter, CancellationToken cancellationToken = default)
        {
            var query = _context.Leads.AsNoTracking()
                .Include(l => l.Employee)
                .Include(l => l.LeadPotentialLevel)
                .Include(l => l.LeadStage)
                .AsSplitQuery()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var searchTerm = filter.Keyword.ToLower();
                query = query.Where(lead =>
                    lead.LeadName.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(l => l.CreateDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Lead>(items, totalCount, filter.PageNumber, filter.PageSize);
        }
    }
}
