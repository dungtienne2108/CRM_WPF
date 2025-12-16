using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Database.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task AssignInvoiceCodeToInstallmentScheduleAsync(int installmentScheduleId, string invoiceCode)
        {
            await _context.InstallmentSchedules
                .Where(i => i.InstallmentId == installmentScheduleId)
                .ExecuteUpdateAsync(i => i.SetProperty(
                    s => s.InvoiceNumber,
                    s => invoiceCode));
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.PaymentMethod)
                .Include(p => p.Customer)
                .Include(p => p.Invoice)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.PaymentId == id);
        }

        public async Task<PagedResult<Payment>> GetPaymentsAsync(PaymentFilter filter)
        {
            var query = _context.Payments
                .Include(p => p.PaymentMethod)
                .Include(p => p.Customer)
                .Include(p => p.Invoice)
                .AsNoTracking()
                .AsSplitQuery()
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(p => p.PaymentCode.Contains(filter.Keyword));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.PaymentDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Payment>(items, totalCount, filter.PageNumber, filter.PageSize);
        }
    }
}
