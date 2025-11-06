using CRM.Application.Dtos.Customer;
using CRM.Application.Dtos.Lead;
using CRM.Shared.Results;

namespace CRM.Application.Interfaces.Customers
{
    public interface ICustomerService
    {
        Task<Result<CustomerDto>> GetCustomerById(int id);
        Task<Result<CustomerDto>> AddCustomerAsync(AddCustomerRequest newCustomer);
        Task<PagedResult<CustomerDto>> GetAllCustomersAsync(GetCustomerRequest request);
        Task<List<SalutationOption>> GetAllSalutationsAsync();
        Task<List<CustomerTypeOption>> GetAllCustomerTypeAsync();
        Task AddContactAsync(int customerId, AddContactRequest newContact);
        Task AddContactRangeAsync(int customerId, List<AddContactRequest> newContacts);
        Task<Result<CustomerDto>> UpdateCustomerAsync(UpdateCustomerRequest updatedCustomer);
        Task<Result> DeleteCustomerAsync(int customerId);
    }
}
