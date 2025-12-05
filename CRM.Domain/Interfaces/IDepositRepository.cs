using CRM.Domain.Filters;
using CRM.Domain.Models;
using CRM.Shared.Results;

namespace CRM.Domain.Interfaces
{
    public interface IDepositRepository : IRepository<Deposit>
    {
        Task<PagedResult<Deposit>> GetDepositsAsync(DepositFilter filter);
        Task<Deposit?> GetDepositByIdAsync(int depositId);
        Task<IEnumerable<Deposit>> GetDepositsByCustomerIdAsync(int customerId);
        Task<Deposit?> GetDepositByProductIdAsync(int productId);
    }
}
