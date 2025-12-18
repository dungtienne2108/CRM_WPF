using CRM.Application.Dtos.Project;
using CRM.Shared.Results;

namespace CRM.Application.Interfaces.Project
{
    public interface IProjectService
    {
        Task<Result<int>> CreateProjectAsync(CreateProjectRequest request);
        Task<Result<int>> CreateProductAsync(CreateProductRequest request);
        Task<Result<ProductDto>> UpdateProductAsync(UpdateProductRequest request);
        Task<Result> UpdateProductStatusAsync(int opportunityId, int productId, int productStatusId);
        Task<Result> DeleteProductAsync(int productId);
        Task<Result<IEnumerable<ProductDto>>> GetProductByOpportunityIdAsync(int opportunityId);
        Task<Result<ProjectDto?>> GetProjectByIdAsync(int projectId);
        Task<Result<ProductDto?>> GetProductByIdAsync(int productId);
        Task<PagedResult<ProjectDto>> GetProjectsAsync(GetProjectRequest request);
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<IEnumerable<ProductDto>> GetUnsoldProductsByProjectIdAsync(int projectId);
        Task<IEnumerable<ProductDto>> GetProductsByProjectIdAsync(int projectId);
        Task<Result<IEnumerable<ProductTypeOption>>> GetProductTypesAsync();
        Task<Result<IEnumerable<ProductStatusOption>>> GetProductStatusesAsync();
    }
}
