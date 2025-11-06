using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Database.Repositories
{
    public sealed class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(AppDbContext context) : base(context)
        {
        }

        public Task<Account?> GetByEmployeeIdAsync(int employeeId)
        {
            return _context.Accounts
                .Include(a => a.Employee)
                .Include(a => a.AccountType)
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId);
        }

        public async Task<Account?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .Include(a => a.Employee)
                .Include(a => a.AccountType)
                .FirstOrDefaultAsync(a => a.AccountName == userName, cancellationToken);
        }
    }
}
