using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Database.Repositories
{
    public sealed class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Customer>> GetAllCustomersAsync(CustomerFilter filter)
        {
            var query = _context.Customers
                .Include(c => c.Gender)
                .Include(c => c.CustomerType)
                .AsSplitQuery()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var searchTerm = filter.Keyword.ToLower();
                query = query.Where(lead =>
                    lead.CustomerName.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(l => l.CreateDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Customer>(items, totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .Include(c => c.Gender)
                .Include(c => c.CustomerType)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer != null)
                await _context.Entry(customer).ReloadAsync();

            return customer;
        }
    }
}
