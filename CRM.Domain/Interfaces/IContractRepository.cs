using CRM.Domain.Filters;
using CRM.Domain.Models;
using CRM.Shared.Results;

namespace CRM.Domain.Interfaces
{
    public interface IContractRepository : IRepository<Contract>
    {
        Task<Contract?> GetContractByIdAsync(int contractId);
        Task<Contract?> GetContractByDepositIdAsync(int depositId);
        Task<PagedResult<Contract>> GetContractsAsync(ContractFilter filter);
        Task<IEnumerable<Contract>> GetContractsByCustomerIdAsync(int customerId);
        Task<IEnumerable<InstallmentSchedule>> GetInstallmentSchedulesByContractIdAsync(int contractId);
    }
}
