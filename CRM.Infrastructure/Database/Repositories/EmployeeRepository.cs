using CRM.Domain.Filters.Base;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Database.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Employee>> GetAllEmployeesAsync(EmployeeFilter filter)
        {
            var query = _context.Employees
                .Include(e => e.Gender)
                .Include(e => e.EmployeeLevel)
                .AsNoTracking()
                .AsSplitQuery()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var searchTerm = filter.Keyword.ToLower();
                query = query.Where(e =>
                    e.EmployeeName.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(e => e.CreateDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Employee>(items, totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeEmail == email);
        }

        public async Task<Employee?> GetByIdentityCardAsync(string identityCard)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeIdentityCard == identityCard);
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            var entity = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (entity != null)
                await _context.Entry(entity).ReloadAsync();

            return entity;
        }
    }
}
