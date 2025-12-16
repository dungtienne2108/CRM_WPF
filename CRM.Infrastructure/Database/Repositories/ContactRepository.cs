using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Database.Repositories
{
    public class ContactRepository : Repository<Contact>, IContactRepository
    {
        public ContactRepository(AppDbContext context) : base(context)
        {
        }

        public async Task AssignContactToCustomerAsync(int contactId, int customerId)
        {
            await _context.CustomerContacts.AddAsync(new CustomerContact
            {
                ContactId = contactId,
                CustomerId = customerId
            });
        }

        public async Task<Contact?> GetContactByIdAsync(int contactId)
        {
            return await _context.Contacts
                .AsNoTracking()
                .Include(c => c.ContactSalutation)
                .Include(c => c.ContactType)
                .Include(c => c.CustomerContacts)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.ContactId == contactId);
        }

        public async Task<IEnumerable<ContactSalutation>> GetContactSalutationsAsync()
        {
            return await _context.ContactSalutations
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<ContactType>> GetContactTypesAsync()
        {
            return await _context.ContactTypes
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PagedResult<Contact>> GetContactsAsync(ContactFilter filter)
        {
            var query = _context.Contacts
                .AsNoTracking()
                .Include(c => c.ContactSalutation)
                .AsSplitQuery()
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                var searchTerm = filter.Keyword.ToLower();
                query = query.Where(c =>
                    c.ContactName.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CreateDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Contact>(items, totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<IEnumerable<Contact>> GetContactsByCustomerIdAsync(int customerId)
        {
            // contact và customer nhiều nhiều
            return await _context.Contacts
                .Include(c => c.ContactType)
                .Where(c => c.CustomerContacts.Any(cc => cc.CustomerId == customerId))
                .ToListAsync();
        }
    }
}
