using CRM.Application.Dtos.Lead;
using CRM.Shared.Results;

namespace CRM.Application.Interfaces.Leads
{
    public interface ILeadService
    {
        Task<Result<LeadDto>> GetLeadByIdAsync(int leadId);
        Task<Result<LeadDto>> UpdateLeadAsync(UpdateLeadStageRequest request);
        Task<Result<LeadDto>> UpdateLeadAsync(UpdateLeadRequest request);
        Task<Result<LeadDto>> UpdateLeadStageAsync(int leadId, int stageId);
        Task<Result<LeadDto>> AddLeadAsync(AddLeadRequest request);
        Task<PagedResult<LeadDto>> GetAllLeadsAsync(GetLeadRequest request);
        Task<IEnumerable<LeadPotentialLevelDto>> GetAllLeadPotentialLevelsAsync();
        Task<IEnumerable<LeadStageDto>> GetAllLeadStagesAsync();
        Task<IEnumerable<LeadSourceDto>> GetAllLeadSourcesAsync();
        Task<Result> DeleteLeadAsync(int leadId);
        Task<Result> AddItemToLeadAsync(int leadId, int productId);
        Task<Result> RemoveProductFromLeadAsync(int leadId, int productId);
    }
}
