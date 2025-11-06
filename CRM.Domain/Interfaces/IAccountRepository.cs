using CRM.Domain.Models;

namespace CRM.Domain.Interfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account?> GetByEmployeeIdAsync(int customerId);
        Task<Account?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    }
}
