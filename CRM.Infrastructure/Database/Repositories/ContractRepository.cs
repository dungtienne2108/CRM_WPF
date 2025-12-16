using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Database.Repositories
{
    public class ContractRepository : Repository<Contract>, IContractRepository
    {
        public ContractRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Contract?> GetContractByDepositIdAsync(int depositId)
        {
            return await _context.Contracts
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.DepositId == depositId);
        }

        public async Task<Contract?> GetContractByIdAsync(int contractId)
        {
            return await _context.Contracts
                .AsNoTracking()
                .Include(c => c.Customer)
                .Include(c => c.Employee)
                .Include(c => c.Deposit)
                .Include(c => c.Product)
                .Include(c => c.ContractStage)
                .Include(c => c.ContractType)
                .Include(c => c.ContractDocuments)
                .Include(c => c.InstallmentSchedules)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.ContractId == contractId);
        }

        public async Task<PagedResult<Contract>> GetContractsAsync(ContractFilter filter)
        {
            var query = _context.Contracts
                .AsNoTracking()
                .Include(c => c.Customer)
                .Include(c => c.Employee)
                .Include(c => c.Deposit)
                .Include(c => c.ContractStage)
                .AsSplitQuery()
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                var searchTerm = filter.Keyword.ToLower();
                query = query.Where(c =>
                    c.ContractName.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CreateDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new(items, totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<IEnumerable<Contract>> GetContractsByCustomerIdAsync(int customerId)
        {
            return await _context.Contracts
                .AsNoTracking()
                .Include(c => c.Customer)
                .Include(c => c.Employee)
                .Include(c => c.Deposit)
                    .ThenInclude(d => d.Product)
                        .ThenInclude(p => p.Project)
                .AsSplitQuery()
                .Where(c => c.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<InstallmentSchedule>> GetInstallmentSchedulesByContractIdAsync(int contractId)
        {
            return await _context.InstallmentSchedules
                .AsNoTracking()
                .Where(i => i.ContractId == contractId)
                .ToListAsync();
        }
    }
}
