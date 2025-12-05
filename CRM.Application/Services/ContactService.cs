using AutoMapper;
using CRM.Application.Dtos.Contact;
using CRM.Application.Interfaces.Contact;
using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.Extensions.Caching.Memory;

namespace CRM.Application.Services
{
    public class ContactService(
        IMapper mapper,
        IMemoryCache memoryCache,
        IContactRepository contactRepository,
        IUnitOfWork unitOfWork) : IContactService
    {
        public async Task<Result<int>> CreateContactAsync(CreateContactRequest request)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                var contact = new Contact
                {
                    ContactName = request.Name,
                    ContactSalutationId = request.SalutationId,
                    ContactEmail = request.Email,
                    ContactPhone = request.Phone,
                    ContactAddress = request.Address,
                    ContactDescription = request.Description,
                    ContactTypeId = request.ContactTypeId
                };

                await contactRepository.AddAsync(contact);
                var added = await unitOfWork.SaveChangesAsync();

                if (request.CustomerId.HasValue)
                {
                    await contactRepository.AssignContactToCustomerAsync(contact.ContactId, request.CustomerId.Value);
                    await unitOfWork.SaveChangesAsync();

                }

                await unitOfWork.CommitTransactionAsync();
                return Result.Success(contact.ContactId);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync();
                return Result.Failure<int>(new("CREATE_CONTACT_ERROR", $"Lỗi xảy ra khi tạo liên hệ: {ex.Message}"));
            }
        }

        public async Task<Result> DeleteContactAsync(int contactId)
        {
            try
            {
                var contact = await contactRepository.GetByIdAsync(contactId);
                if (contact == null)
                {
                    return Result.Failure(new Error("CONTACT_NOT_FOUND", $"Không tìm thấy liên hệ với Id: {contactId}"));
                }

                contactRepository.Remove(contact);
                var deleted = await unitOfWork.SaveChangesAsync();
                if (deleted > 0)
                {
                    return Result.Success();
                }
                else
                {
                    return Result.Failure(new Error("DELETE_CONTACT_FAILED", "Xóa liên hệ thất bại"));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("DELETE_CONTACT_ERROR", $"Lỗi xảy ra khi xóa liên hệ: {ex.Message}"));
            }
        }

        public async Task<Result<ContactDto?>> GetContactByIdAsync(int contactId)
        {
            //if (memoryCache.TryGetValue($"Contact_{contactId}", out ContactDto? cachedContact))
            //{
            //    return Result.Success(cachedContact);
            //}

            var contact = await contactRepository.GetContactByIdAsync(contactId);

            if (contact == null)
            {
                return Result.Failure<ContactDto?>(new Error("CONTACT_NOT_FOUND", $"Không tìm thấy liên hệ với Id: {contactId}"));
            }

            var contactDto = mapper.Map<ContactDto>(contact);

            //memoryCache.Set($"Contact_{contactId}", contactDto, TimeSpan.FromMinutes(10));

            return Result.Success<ContactDto?>(contactDto);
        }

        public async Task<IEnumerable<ContactSalutationOptions>> GetContactSalutationOptionsAsync()
        {
            var contactSalutations = await contactRepository.GetContactSalutationsAsync();

            var salutationOptions = contactSalutations.Select(s => new ContactSalutationOptions
            {
                Id = s.ContactSalutationId,
                Name = s.ContactSalutationName
            });

            return salutationOptions;
        }

        public async Task<PagedResult<ContactDto>> GetContactsAsync(GetContactRequest request)
        {
            //if (memoryCache.TryGetValue($"Contacts_{request.Keyword}_{request.PageNumber}_{request.PageSize}", out PagedResult<ContactDto>? cachedContacts))
            //{
            //    return cachedContacts;
            //}

            var contactFilter = new ContactFilter
            {
                Keyword = request.Keyword,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };

            var contacts = await contactRepository.GetContactsAsync(contactFilter);

            var contactDtos = mapper.Map<IEnumerable<ContactDto>>(contacts.Items);

            //memoryCache.Set($"Contacts_{request.Keyword}_{request.PageNumber}_{request.PageSize}", new PagedResult<ContactDto>(contactDtos, contacts.TotalCount, contacts.PageNumber, contacts.PageSize), TimeSpan.FromSeconds(30));

            return new PagedResult<ContactDto>(contactDtos, contacts.TotalCount, contacts.PageNumber, contacts.PageSize);
        }

        public async Task<IEnumerable<ContactDto>> GetContactsByCustomerIdAsync(int customerId)
        {
            //if (memoryCache.TryGetValue($"CustomerContacts_{customerId}", out IEnumerable<ContactDto>? cachedContacts))
            //{
            //    return cachedContacts;
            //}

            var contacts = await contactRepository.GetContactsByCustomerIdAsync(customerId);

            //memoryCache.Set($"CustomerContacts_{customerId}", mapper.Map<IEnumerable<ContactDto>>(contacts), TimeSpan.FromMinutes(10));

            return mapper.Map<IEnumerable<ContactDto>>(contacts);
        }

        public async Task<IEnumerable<ContactTypeOption>> GetContactTypeOptionsAsync()
        {
            var contactTypes = await contactRepository.GetContactTypesAsync();

            var typeOptions = contactTypes.Select(t => new ContactTypeOption
            {
                Id = t.ContactTypeId,
                Name = t.ContactTypeName
            });

            return typeOptions;
        }

        public async Task<Result<ContactDto>> UpdateContactAsync(UpdateContactRequest request)
        {
            try
            {
                //memoryCache.Remove($"Contact_{request.Id}");

                var contact = await contactRepository.GetByIdAsync(request.Id);
                if (contact == null)
                {
                    return Result.Failure<ContactDto>(new("CONTACT_NOT_FOUND", $"Không tìm thấy liên hệ với Id: {request.Id}"));
                }

                contact.ContactName = request.Name;
                contact.ContactEmail = request.Email;
                contact.ContactPhone = request.Phone;
                contact.ContactAddress = request.Address;
                contact.ContactDescription = request.Description;
                contact.ContactSalutationId = request.SalutationId;
                contact.ContactTypeId = request.ContactTypeId;

                if (request.CustomerId.HasValue)
                {
                    await contactRepository.AssignContactToCustomerAsync(contact.ContactId, request.CustomerId.Value);
                }

                contactRepository.Update(contact);
                var updated = await unitOfWork.SaveChangesAsync();
                if (updated > 0)
                {
                    var contactDto = mapper.Map<ContactDto>(contact);
                    return Result.Success(contactDto);
                }
                else
                {
                    return Result.Failure<ContactDto>(new("UPDATE_CONTACT_FAILED", "Cập nhật liên hệ thất bại"));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<ContactDto>(new("UPDATE_CONTACT_ERROR", $"Lỗi xảy ra khi cập nhật liên hệ: {ex.Message}"));
            }
        }
    }
}
