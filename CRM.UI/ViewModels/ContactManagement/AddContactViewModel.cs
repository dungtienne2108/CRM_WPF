using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Contact;
using CRM.Application.Dtos.Customer;
using CRM.Application.Dtos.Lead;
using CRM.Application.Interfaces.Contact;
using CRM.Application.Interfaces.Customers;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.ContactManagement
{
    public partial class AddContactViewModel : ViewModelBase
    {
        private readonly IContactService _contactService;
        private readonly ICustomerService _customerService;

        [ObservableProperty]
        private ObservableCollection<SalutationOption> _salutationOptions = new();
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng chọn danh xưng")]
        private int _salutationId;
        [ObservableProperty]
        private ObservableCollection<ContactTypeOption> _contactTypeOptions = new();
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng chọn loại liên hệ")]
        private int _contactTypeId;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng nhập tên liên hệ")]
        private string _contactName = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [EmailAddress(ErrorMessage = "Vui lòng nhập email hợp lệ")]
        [Required(ErrorMessage = "Vui lòng nhập email liên hệ")]
        private string _contactEmail = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Phone(ErrorMessage = "Vui lòng nhập số điện thoại hợp lệ")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại liên hệ")]
        private string _contactPhone = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        private string _contactAddress = string.Empty;
        [ObservableProperty]
        private string _contactDescription = string.Empty;

        [ObservableProperty]
        private int? _customerId;

        [ObservableProperty]
        private ObservableCollection<CustomerDto> _customerSuggestions = new();
        [ObservableProperty]
        private CustomerDto? _selectedCustomer;
        [ObservableProperty]
        private string _customerSearchKeyword = string.Empty;
        [ObservableProperty]
        private bool _isCustomerDropdownOpen = false;

        public bool CanSave => !HasAnyErrors;

        public AddContactViewModel(IContactService contactService, ICustomerService customerService)
        {
            _contactService = contactService;
            _customerService = customerService;

            ErrorsChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(CanSave));
            };
        }

        #region Public methods
        public async Task LoadDataAsync()
        {
            await LoadSalutationsAsync();
            await LoadContactTypesAsync();
            await GetCustomersAsync();
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            if (HasErrors)
            {
                return;
            }

            await CreateNewContactAsync();
        }

        [RelayCommand]
        private void Cancel()
        {
            System.Windows.Application.Current.Windows
                .OfType<System.Windows.Window>()
                .SingleOrDefault(x => x.DataContext == this)?
                .Close();
        }
        #endregion

        #region Private methods
        private async Task CreateNewContactAsync()
        {
            var request = new CreateContactRequest
            {
                Name = ContactName,
                SalutationId = SalutationId,
                Email = ContactEmail,
                Phone = ContactPhone,
                Address = ContactAddress,
                Description = ContactDescription,
                ContactTypeId = ContactTypeId
            };

            if (CustomerId.HasValue)
            {
                request.CustomerId = CustomerId.Value;
            }

            var result = await _contactService.CreateContactAsync(request);
            if (result.IsSuccess)
            {
                MessageBox.Show("Thêm liên hệ thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                Cancel();
            }
            else
            {
                MessageBox.Show($"Thêm liên hệ thất bại: {result.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Cancel();
            }
        }

        private async Task LoadSalutationsAsync()
        {
            var options = await _customerService.GetAllSalutationsAsync();
            SalutationOptions = new(options);
        }

        private async Task LoadContactTypesAsync()
        {
            var options = await _contactService.GetContactTypeOptionsAsync();
            ContactTypeOptions = new(options);
        }

        private async Task GetCustomersAsync()
        {
            IsCustomerDropdownOpen = true;
            try
            {
                var getCustomerRequest = new GetCustomerRequest
                {
                    Keyword = CustomerSearchKeyword,
                    PageNumber = 1,
                    PageSize = 1000
                };

                var customersResult = await _customerService.GetAllCustomersAsync(getCustomerRequest);

                CustomerSuggestions.Clear();

                foreach (var customer in customersResult.Items)
                {
                    CustomerSuggestions.Add(customer);
                }
            }
            catch (Exception)
            {
                // Log or handle the exception as needed
            }
        }
        #endregion

        #region property changed methods
        partial void OnCustomerSearchKeywordChanged(string value)
        {
            if (SelectedCustomer != null && SelectedCustomer.Name != value)
            {
                _ = GetCustomersAsync();
            }
        }

        partial void OnSelectedCustomerChanged(CustomerDto? value)
        {
            if (value != null)
            {
                CustomerId = value.Id;
                CustomerSearchKeyword = value.Name;
                IsCustomerDropdownOpen = false;
            }
        }
        #endregion
    }
}
