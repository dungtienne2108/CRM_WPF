using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Contact;
using CRM.Application.Dtos.Customer;
using CRM.Application.Dtos.Lead;
using CRM.Application.Interfaces.Contact;
using CRM.Application.Interfaces.Customers;
using CRM.Application.Interfaces.Employee;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.ContactManagement
{
    public partial class ContactDetailViewModel : ViewModelBase
    {
        private readonly IContactService _contactService;
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        private int _contactId;
        [ObservableProperty]
        private string _contactName = string.Empty;
        [ObservableProperty]
        private string _contactEmail = string.Empty;
        [ObservableProperty]
        private string _contactPhone = string.Empty;
        [ObservableProperty]
        private string _contactAddress = string.Empty;
        [ObservableProperty]
        private string _contactDescription = string.Empty;
        [ObservableProperty]
        private string _salutationName = string.Empty;
        [ObservableProperty]
        private int _salutationId;
        [ObservableProperty]
        private string _contactTypeName = string.Empty;
        [ObservableProperty]
        private int _contactTypeId;
        [ObservableProperty]
        private int? _customerId;
        [ObservableProperty]
        private string? _customerName = string.Empty;

        [ObservableProperty]
        private ObservableCollection<SalutationOption> _salutationOptions = new();
        [ObservableProperty]
        private ObservableCollection<ContactTypeOption> _contactTypeOptions = new();

        [ObservableProperty]
        private ObservableCollection<CustomerDto> _customerOptions = new();
        [ObservableProperty]
        private CustomerDto? _selectedCustomer;
        [ObservableProperty]
        private string _customerSearchKeyword = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeleteVisible))]
        private bool _isEditMode = false;

        public bool IsDeleteVisible => !IsEditMode;

        public ContactDetailViewModel(IContactService contactService,
            ICustomerService customerService,
            IEmployeeService employeeService)
        {
            _contactService = contactService;
            _customerService = customerService;
            _employeeService = employeeService;
        }

        #region Public Methods
        public async Task LoadDataAsync(int contactId)
        {
            if (contactId <= 0)
            {
                return;
            }

            ContactId = contactId;
            await GetContactAsync(contactId);
            await LoadSalutationOptionsAsync();
            await LoadContactTypeOptionsAsync();
            await GetCustomersAsync();
        }
        #endregion

        #region Commands
        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            if (HasErrors)
            {
                return;
            }

            await UpdateContactAsync(ContactId);
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            try
            {
                var res = MessageBox.Show("Bạn có chắc chắn xóa liên hệ?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res == MessageBoxResult.Yes)
                {
                    var deleteResult = await _contactService.DeleteContactAsync(ContactId);

                    if (deleteResult.IsSuccess)
                    {
                        MessageBox.Show("Xóa thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                        System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại", "Thấy bại", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch { MessageBox.Show("Xóa thất bại", "Thấy bại", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        #endregion

        #region Private Methods
        private async Task UpdateContactAsync(int contactId)
        {
            try
            {
                var updateDto = new Application.Dtos.Contact.UpdateContactRequest
                {
                    Id = contactId,
                    Name = ContactName,
                    Email = ContactEmail,
                    Phone = ContactPhone,
                    Address = ContactAddress,
                    Description = ContactDescription,
                    SalutationId = SalutationId,
                    ContactTypeId = ContactTypeId,
                    CustomerId = CustomerId
                };
                var updateResult = await _contactService.UpdateContactAsync(updateDto);
                if (updateResult.IsSuccess)
                {
                    IsEditMode = false;
                    MessageBox.Show("Cập nhật thông tin liên hệ thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetContactAsync(contactId);
                }
                else
                {
                    MessageBox.Show($"Cập nhật thông tin liên hệ thất bại: {updateResult.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task GetCustomersAsync()
        {
            try
            {
                var getCustomerRequest = new GetCustomerRequest
                {
                    Keyword = CustomerSearchKeyword,
                    PageNumber = 1,
                    PageSize = 1000
                };
                var getCustomerResult = await _customerService.GetAllCustomersAsync(getCustomerRequest);

                CustomerOptions.Clear();
                foreach (var customer in getCustomerResult.Items)
                {
                    CustomerOptions.Add(customer);
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task GetContactAsync(int contactId)
        {
            try
            {
                var contactResult = await _contactService.GetContactByIdAsync(contactId);
                if (contactResult.IsSuccess)
                {
                    var contact = contactResult.Value;
                    ContactName = contact.Name;
                    ContactEmail = contact.Email;
                    ContactPhone = contact.Phone;
                    ContactAddress = contact.Address;
                    ContactDescription = contact.Description ?? string.Empty;
                    SalutationId = contact.SalutationId;
                    SalutationName = contact.Salutation ?? string.Empty;
                    ContactTypeId = contact.ContactTypeId;
                    ContactTypeName = contact.ContactType ?? string.Empty;

                    CustomerId = contact.CustomerId;
                    var customerResult = await _customerService.GetCustomerById(contact.CustomerId.Value);
                    if (customerResult.IsFailure)
                    {
                        MessageBox.Show($"Lấy thông tin khách hàng thất bại: {customerResult.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    CustomerName = customerResult.Value.Name;
                }
            }
            catch (Exception)
            {

            }
        }

        private async Task LoadSalutationOptionsAsync()
        {
            try
            {
                var salutationResult = await _customerService.GetAllSalutationsAsync();

                SalutationOptions.Clear();

                foreach (var salutation in salutationResult)
                {
                    SalutationOptions.Add(salutation);
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task LoadContactTypeOptionsAsync()
        {
            try
            {
                var contactTypeResult = await _contactService.GetContactTypeOptionsAsync();
                ContactTypeOptions.Clear();
                foreach (var contactType in contactTypeResult)
                {
                    ContactTypeOptions.Add(contactType);
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region Property changed
        partial void OnCustomerSearchKeywordChanged(string value)
        {
            if (SelectedCustomer != null && SelectedCustomer.Name != value)
            {
                SelectedCustomer = null;
                CustomerId = null;
                CustomerName = string.Empty;
                _ = GetCustomersAsync();
            }
        }

        partial void OnSelectedCustomerChanged(CustomerDto? value)
        {
            if (value != null)
            {
                CustomerId = value.Id;
                CustomerName = value.Name;
                CustomerSearchKeyword = value.Name;
            }
            else
            {
                CustomerId = null;
                CustomerName = string.Empty;
            }
        }
        #endregion
    }
}
