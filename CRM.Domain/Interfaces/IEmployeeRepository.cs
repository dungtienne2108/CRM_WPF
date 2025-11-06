using CRM.Domain.Filters.Base;
using CRM.Domain.Models;
using CRM.Shared.Results;

namespace CRM.Domain.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee?> GetByEmailAsync(string email);
        Task<Employee?> GetByIdentityCardAsync(string identityCard);
        Task<PagedResult<Employee>> GetAllEmployeesAsync(EmployeeFilter filter);
    }
}
