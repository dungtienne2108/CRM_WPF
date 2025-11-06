using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Database.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.ProductStatus)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<IEnumerable<Product>> GetProductsByProjectIdAsync(int projectId)
        {
            var products = await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.ProductStatus)
                .Where(p => p.ProjectId == projectId && p.ProductStatusId != 3)
                .AsSplitQuery()
                .ToListAsync();

            return products;
        }
    }
}
