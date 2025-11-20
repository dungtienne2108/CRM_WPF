using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Database.Repositories
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Contract)
                .Include(i => i.InstallmentSchedule)
                .AsSplitQuery()
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice != null)
                await _context.Entry(invoice).ReloadAsync();

            return invoice;
        }

        public async Task<PagedResult<Invoice>> GetInvoicesAsync(InvoiceFilter filter)
        {
            var query = _context.Invoices
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Customer)
                .AsSplitQuery()
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                var searchTerm = filter.Keyword.ToLower();
                query = query.Where(i =>
                    i.InvoiceCode.ToLower().Contains(searchTerm));
            }

            if (filter.IsPaid.HasValue)
            {
                query = query.Where(i => i.Status == InvoiceStatus.Paid);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(i => i.CreateDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Invoice>(items, totalCount, filter.PageNumber, filter.PageSize);
        }
    }
}
