using CRM.Domain.Models;

namespace CRM.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetUnsoldProductsByProjectIdAsync(int projectId);
        Task<IEnumerable<Product>> GetProductsByProjectIdAsync(int projectId);
        Task<Product?> GetProductByIdAsync(int productId);
        Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<int> productIds);
        Task UpdateProductStatusByIdsAsync(IEnumerable<int> productIds, int newStatus);
        Task UpdateProductStatusByIdAsync(int productId, int newStatus);
    }
}
