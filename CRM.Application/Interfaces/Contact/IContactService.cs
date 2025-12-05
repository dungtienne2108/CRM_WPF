using CRM.Application.Dtos.Contact;
using CRM.Shared.Results;

namespace CRM.Application.Interfaces.Contact
{
    public interface IContactService
    {
        Task<Result<ContactDto>> UpdateContactAsync(UpdateContactRequest request);
        Task<Result<int>> CreateContactAsync(CreateContactRequest request);
        Task<PagedResult<ContactDto>> GetContactsAsync(GetContactRequest request);
        Task<IEnumerable<ContactDto>> GetContactsByCustomerIdAsync(int customerId);
        Task<Result<ContactDto?>> GetContactByIdAsync(int contactId);
        Task<Result> DeleteContactAsync(int contactId);
        Task<IEnumerable<ContactSalutationOptions>> GetContactSalutationOptionsAsync();
        Task<IEnumerable<ContactTypeOption>> GetContactTypeOptionsAsync();
    }
}
