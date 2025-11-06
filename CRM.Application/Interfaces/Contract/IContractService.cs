using CRM.Application.Dtos.Contract;
using CRM.Shared.Results;

namespace CRM.Application.Interfaces.Contract
{
    public interface IContractService
    {
        Task<Result<ContractDto>> UpdateContractAsync(UpdateContractRequest request);
        Task<Result<int>> CreateContractAsync(CreateContractRequest request);
        Task<Result<ContractDto>> GetContractByIdAsync(int contractId);
        Task<Result<IEnumerable<ContractDto>>> GetContractsByCustomerIdAsync(int customerId);
        Task<PagedResult<ContractDto>> GetContractsAsync(GetContractRequest request);
        Task<Result<IEnumerable<ContractTypeOption>>> GetContractTypesAsync();
        Task<Result<IEnumerable<ContractStageOption>>> GetContractStagesAsync();
        Task<Result> DeleteContractAsync(int contractId);
        Task<Result> UploadContractImageAsync(int contractId, ContractDocumentDto contractDocument);
        Task<Result> RemoveContractImageAsync(int contractId, int documentId);
    }
}
