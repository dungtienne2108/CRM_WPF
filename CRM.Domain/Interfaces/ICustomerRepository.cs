using CRM.Domain.Filters;
using CRM.Domain.Models;
using CRM.Shared.Results;

namespace CRM.Domain.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<PagedResult<Customer>> GetAllCustomersAsync(CustomerFilter filter);
        Task<Customer?> GetCustomerByIdAsync(int id);
    }
}
