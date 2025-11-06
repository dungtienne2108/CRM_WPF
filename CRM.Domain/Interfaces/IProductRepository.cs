using CRM.Domain.Models;

namespace CRM.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByProjectIdAsync(int projectId);
        Task<Product?> GetProductByIdAsync(int productId);
    }
}
