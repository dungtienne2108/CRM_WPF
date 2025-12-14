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

        public async Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<int> productIds)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductType)
                .Include(p => p.ProductStatus)
                .Where(p => productIds.Contains(p.ProductId))
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByProjectIdAsync(int projectId)
        {
            var products = await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.ProductStatus)
                .Where(p => p.ProjectId == projectId && p.ProductStatusId == 1)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();

            return products;
        }

        public async Task UpdateProductStatusByIdsAsync(IEnumerable<int> productIds, int newStatus)
        {
            await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.ProductStatusId, newStatus));
        }

        public async Task UpdateProductStatusByIdAsync(int productId, int newStatus)
        {
            await _context.Products
                .Where(p => p.ProductId == productId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.ProductStatusId, newStatus));
        }
    }
}
