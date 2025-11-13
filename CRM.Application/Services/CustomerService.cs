using AutoMapper;
using CRM.Application.Dtos.Customer;
using CRM.Application.Dtos.Lead;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Customers;
using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.Extensions.Caching.Memory;

namespace CRM.Application.Services
{
    public sealed class CustomerService(
            ISessionService sessionService,
            ICustomerRepository customerRepository,
            IRepository<ContactSalutation> salutationRepository,
            IRepository<Contact> contactRepository,
            IRepository<CustomerType> customerTypeRepository,
            IRepository<CustomerContact> customerContactRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IMemoryCache memoryCache) : ICustomerService
    {

        public async Task AddContactAsync(int customerId, AddContactRequest newContact)
        {
            var contact = new Contact
            {
                ContactName = newContact.ContactName,
                ContactPhone = newContact.ContactPhone,
                ContactEmail = newContact.ContactEmail,
                ContactAddress = newContact.ContactAddress,
                ContactSalutationId = newContact.ContactSalutationId,
                ContactDescription = newContact.ContactDescription,
                CreateDate = DateTime.UtcNow,
            };


            var customerContact = new CustomerContact
            {
                CustomerId = customerId,
                Contact = contact,
                Role = newContact.ContactRole
            };

            contact.CustomerContacts = new List<CustomerContact> { customerContact };

            //memoryCache.Remove($"Customer_{customerContact.CustomerId}");
            //memoryCache.Remove($"CustomerContacts_{customerId}");

            await contactRepository.AddAsync(contact);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task AddContactRangeAsync(int customerId, List<AddContactRequest> newContacts)
        {
            var contacts = newContacts.Select(newContact => new Contact
            {
                ContactName = newContact.ContactName,
                ContactPhone = newContact.ContactPhone,
                ContactEmail = newContact.ContactEmail,
                ContactAddress = newContact.ContactAddress,
                ContactSalutationId = newContact.ContactSalutationId,
                ContactDescription = newContact.ContactDescription,
                CreateDate = DateTime.UtcNow,
                CustomerContacts = new List<CustomerContact>
                {
                    new CustomerContact
                    {
                        CustomerId = customerId,
                        Role = newContact.ContactRole
                    }
                }
            }).ToList();

            memoryCache.Remove($"Customer_{customerId}");
            memoryCache.Remove($"CustomerContacts_{customerId}");

            await contactRepository.AddRangeAsync(contacts);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<Result<CustomerDto>> AddCustomerAsync(AddCustomerRequest newCustomer)
        {
            try
            {
                var existingCustomerByName = await customerRepository.AnyAsync(c => c.CustomerName == newCustomer.Name);
                if (existingCustomerByName)
                {
                    return Result.Failure<CustomerDto>(new Error("DUPLICATE_CUSTOMER_NAME", $"Khách hàng tên '{newCustomer.Name}' đã tồn tại."));
                }

                var existingCustomerByIdentityCard = await customerRepository.AnyAsync(c => c.CustomerIdentityCard == newCustomer.IdentityCard);
                if (existingCustomerByIdentityCard)
                {
                    return Result.Failure<CustomerDto>(new Error("DUPLICATE_IDENTITY_CARD", $"Số CMND/CCCD '{newCustomer.IdentityCard}' đã tồn tại."));
                }

                var currentEmployee = sessionService.CurrentAccount;

                var customer = new Customer
                {
                    CustomerName = newCustomer.Name,
                    CustomerIdentityCard = newCustomer.IdentityCard,
                    CustomerPhone = newCustomer.Phone,
                    CustomerEmail = newCustomer.Email,
                    CustomerDescription = newCustomer.Description,
                    CustomerAddress = newCustomer.Address,
                    CustomerTypeId = newCustomer.CustomerTypeId,
                };

                if (newCustomer.EmployeeId.HasValue)
                {
                    customer.EmployeeId = newCustomer.EmployeeId;
                    customer.CreateBy = newCustomer.EmployeeId.ToString();
                }

                if (newCustomer.GenderId.HasValue)
                {
                    customer.GenderId = newCustomer.GenderId;
                }

                await customerRepository.AddAsync(customer);
                var added = await unitOfWork.SaveChangesAsync();

                if (added > 0)
                {
                    var customerDto = mapper.Map<CustomerDto>(customer);
                    return Result.Success(customerDto);
                }
                else
                {
                    return Result.Failure<CustomerDto>(new Error("ADD_CUSTOMER_FAILED", "Thêm khách hàng thất bại."));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<CustomerDto>(new Error("ADD_CUSTOMER_ERROR", $"Lỗi xảy ra khi thêm khách hàng: {ex.Message}"));
            }
        }

        public async Task<Result> DeleteCustomerAsync(int customerId)
        {
            try
            {
                var customer = await customerRepository.GetByIdAsync(customerId);

                if (customer == null)
                {
                    return Result.Failure(new("customer_not_found", "Không tìm thấy khách hàng"));
                }

                customerRepository.Remove(customer);
                var deleted = await unitOfWork.SaveChangesAsync();
                if (deleted > 0)
                {
                    return Result.Success();
                }

                return Result.Failure(new("failed", "Lỗi khi xóa khách hnafg"));
            }
            catch { throw; }
        }

        public async Task<PagedResult<CustomerDto>> GetAllCustomersAsync(GetCustomerRequest request)
        {
            try
            {
                var filter = new CustomerFilter
                {
                    Keyword = request.Keyword,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                var customers = await customerRepository.GetAllCustomersAsync(filter);

                var customerDtos = mapper.Map<IEnumerable<CustomerDto>>(customers.Items);

                var pagedResult = new PagedResult<CustomerDto>(customerDtos, customers.TotalCount, customers.PageSize, customers.PageNumber);

                //memoryCache.Set($"CustomerList_{request.Keyword}_{request.PageNumber}_{request.PageSize}", pagedResult, TimeSpan.FromMinutes(1));

                return pagedResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi xảy ra khi lấy danh sách khách hàng: {ex.Message}", ex);
            }
        }

        public async Task<List<CustomerTypeOption>> GetAllCustomerTypeAsync()
        {
            var customerTypes = await customerTypeRepository.GetAllAsync();

            var customerTypeOptions = customerTypes
                .Select(ct => new CustomerTypeOption
                {
                    Id = ct.CustomerTypeId,
                    Name = ct.CustomerTypeName
                })
                .ToList();

            memoryCache.Set("CustomerTypeOptions", customerTypeOptions, TimeSpan.FromHours(1));

            return customerTypeOptions;
        }

        public async Task<List<SalutationOption>> GetAllSalutationsAsync()
        {
            var salutations = await salutationRepository.GetAllAsync();
            var salutationOptions = salutations
                .Select(s => new SalutationOption
                {
                    Id = s.ContactSalutationId,
                    Name = s.ContactSalutationName
                })
                .ToList();

            memoryCache.Set("SalutationOptions", salutationOptions, TimeSpan.FromHours(1));

            return salutationOptions;
        }

        public async Task<Result<CustomerDto>> GetCustomerById(int id)
        {
            try
            {
                var customer = await customerRepository.GetCustomerByIdAsync(id);

                if (customer == null)
                {
                    return Result.Failure<CustomerDto>(new Error("CUSTOMER_NOT_FOUND", $"Khách hàng với id : {id} không tồn tại"));
                }

                var customerDto = mapper.Map<CustomerDto>(customer);

                memoryCache.Set($"Customer_{id}", customerDto, TimeSpan.FromMinutes(10));

                return Result.Success(customerDto);
            }
            catch (Exception ex)
            {
                return Result.Failure<CustomerDto>(new Error("GET_CUSTOMER_ERROR", $"Lỗi xảy ra khi lấy thông tin khách hàng: {ex.Message}"));
            }
        }

        public async Task<Result<CustomerDto>> UpdateCustomerAsync(UpdateCustomerRequest updatedCustomer)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(updatedCustomer.Name))
                {
                    return Result.Failure<CustomerDto>(new Error("INVALID_CUSTOMER_NAME", "Tên khách hàng không được để trống."));
                }

                if (string.IsNullOrEmpty(updatedCustomer.PhoneNumber))
                {
                    return Result.Failure<CustomerDto>(new Error("INVALID_PHONE_NUMBER", "Số điện thoại không được để trống."));
                }

                var customer = await customerRepository.GetByIdAsync(updatedCustomer.Id);

                if (customer == null)
                {
                    return Result.Failure<CustomerDto>(new Error("CUSTOMER_NOT_FOUND", $"Khách hàng với id : {updatedCustomer.Id} không tồn tại"));
                }

                if (updatedCustomer.Name is not null)
                {
                    customer.CustomerName = updatedCustomer.Name;
                }

                if (updatedCustomer.PhoneNumber is not null)
                {
                    customer.CustomerPhone = updatedCustomer.PhoneNumber;
                }

                if (updatedCustomer.Email is not null)
                {
                    customer.CustomerEmail = updatedCustomer.Email;
                }

                if (updatedCustomer.Address is not null)
                {
                    customer.CustomerAddress = updatedCustomer.Address;
                }

                if (updatedCustomer.CustomerTypeId.HasValue)
                {
                    customer.CustomerTypeId = updatedCustomer.CustomerTypeId.Value;
                }

                if (updatedCustomer.IdentityCard is not null)
                {
                    customer.CustomerIdentityCard = updatedCustomer.IdentityCard;
                }

                if (updatedCustomer.Description is not null)
                {
                    customer.CustomerDescription = updatedCustomer.Description;
                }

                if (updatedCustomer.LeadId.HasValue)
                {
                    customer.LeadId = updatedCustomer.LeadId.Value;
                }

                customerRepository.Update(customer);
                var updated = await unitOfWork.SaveChangesAsync();

                if (updated > 0)
                {
                    var customerDto = mapper.Map<CustomerDto>(customer);
                    memoryCache.Remove($"Customer_{customer.CustomerId}");
                    return Result.Success(customerDto);
                }
                else
                {
                    return Result.Failure<CustomerDto>(new Error("UPDATE_CUSTOMER_FAILED", "Cập nhật khách hàng thất bại."));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<CustomerDto>(new Error("UPDATE_CUSTOMER_ERROR", $"Lỗi xảy ra khi cập nhật khách hàng: {ex.Message}"));
            }
        }
    }
}