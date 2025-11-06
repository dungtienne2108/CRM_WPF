using CRM.Application.Dtos.Deposit;
using CRM.Shared.Results;

namespace CRM.Application.Interfaces.Deposit
{
    public interface IDepositService
    {
        Task<Result<DepositDto>> UpdateDepositAsync(UpdateDepositRequest request);
        Task<Result<int>> CreateDepositAsync(CreateDepositRequest request);
        Task<PagedResult<DepositDto>> GetDepositsAsync(GetDepositRequest request);
        Task<Result<DepositDto>> GetDepositByIdAsync(int depositId);
        Task<Result<IEnumerable<DepositDto>>> GetDepositsByCustomerIdAsync(int customerId);
        Task<Result> DeleteDepositAsync(int depositId);
    }
}
