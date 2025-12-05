using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Database.Repositories
{
    public class DepositRepository : Repository<Deposit>, IDepositRepository
    {
        public DepositRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Deposit?> GetDepositByIdAsync(int depositId)
        {
            var deposit = await _context.Deposits
                .Include(c => c.Opportunity)
                .Include(c => c.Customer)
                .Include(c => c.Contact)
                .Include(c => c.Employee)
                .Include(c => c.Product)
                    .ThenInclude(p => p.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(d => d.DepositId == depositId);

            if (deposit != null)
            {
                await _context.Entry(deposit).ReloadAsync();
            }

            return deposit;
        }

        public async Task<Deposit?> GetDepositByProductIdAsync(int productId)
        {
            var deposit = await _context.Deposits
                //.Include(c => c.Opportunity)
                //.Include(c => c.Customer)
                //.Include(c => c.Contact)
                //.Include(c => c.Employee)
                //.Include(c => c.Product)
                //    .ThenInclude(p => p.Project)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(d => d.ProductId == productId);

            return deposit;
        }

        public async Task<PagedResult<Deposit>> GetDepositsAsync(DepositFilter filter)
        {
            var query = _context.Deposits
                .AsNoTracking()
                .Include(c => c.Opportunity)
                .Include(c => c.Customer)
                .Include(c => c.Contact)
                .Include(c => c.Employee)
                .Include(c => c.Product)
                    .ThenInclude(p => p.Project)
                .AsSplitQuery()
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                var keyword = filter.Keyword.Trim().ToLower();
                query = query.Where(d =>
                    d.DepositName != null && d.DepositName.ToLower().Contains(keyword)

                );
            }

            if (filter.IsCreatedContract.HasValue)
            {
                query = query.Where(d => d.IsCreatedContract == filter.IsCreatedContract.Value);
            }

            var totalCount = await query.CountAsync();

            var deposits = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new(deposits, totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<IEnumerable<Deposit>> GetDepositsByCustomerIdAsync(int customerId)
        {
            return await _context.Deposits
                .AsNoTracking()
                .Include(c => c.Opportunity)
                .Include(c => c.Customer)
                .Include(c => c.Contact)
                .Include(c => c.Employee)
                .Include(c => c.Product)
                .AsSplitQuery()
                .Where(d => d.CustomerId == customerId)
                .ToListAsync();
        }
    }
}
