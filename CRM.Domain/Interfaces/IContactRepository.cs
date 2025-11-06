using CRM.Domain.Filters;
using CRM.Domain.Models;
using CRM.Shared.Results;

namespace CRM.Domain.Interfaces
{
    public interface IContactRepository : IRepository<Contact>
    {
        Task AssignContactToCustomerAsync(int contactId, int customerId);
        Task<Contact?> GetContactByIdAsync(int contactId);
        Task<PagedResult<Contact>> GetContactsAsync(ContactFilter filter);
        Task<IEnumerable<Contact>> GetContactsByCustomerIdAsync(int customerId);
    }
}
