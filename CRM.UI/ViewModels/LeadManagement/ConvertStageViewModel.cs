using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Contact;
using CRM.Application.Dtos.Customer;
using CRM.Application.Dtos.Employee;
using CRM.Application.Dtos.Lead;
using CRM.Application.Dtos.Opportunity;
using CRM.Application.Dtos.Project;
using CRM.Application.Interfaces.Contact;
using CRM.Application.Interfaces.Customers;
using CRM.Application.Interfaces.Employee;
using CRM.Application.Interfaces.Opportunity;
using CRM.Application.Interfaces.Project;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.LeadManagement
{
    public partial class ConvertStageViewModel : ViewModelBase
    {
        private readonly ICustomerService _customerService;
        private readonly IOpportunityService _opportunityService;
        private readonly IProjectService _projectService;
        private readonly IEmployeeService _employeeService;
        private readonly IContactService _contactService;

        [ObservableProperty]
        private bool _isDialogVisible;
        [ObservableProperty]
        private bool _isCustomerDropDownOpen;
        [ObservableProperty]
        private bool _isEmployeeDropDownOpen;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNewCustomer))]
        [NotifyPropertyChangedFor(nameof(IsExistingCustomer))]
        private string _selectedTypeAction;

        public bool IsNewCustomer => SelectedTypeAction == "Khách hàng mới";
        public bool IsExistingCustomer => SelectedTypeAction == "Khách hàng cũ";

        [ObservableProperty]
        private int? _leadId;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Tên khách hàng không được để trống.")]
        [MinLength(3, ErrorMessage = "Tên khách hàng phải có ít nhất 3 ký tự.")]
        private string _newCustomerName = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Loại khách hàng không được để trống.")]
        private int _newCustomerTypeId;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        private string _newCustomerPhone = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Số CMND/CCCD không được để trống.")]
        [MinLength(9, ErrorMessage = "Số CMND/CCCD phải có ít nhất 9 ký tự.")]
        private string _newCustomerIdCard = string.Empty;

        [ObservableProperty]
        private string _typeSearch;
        [ObservableProperty]
        private string _customerSearchKeyword = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Khách hàng phải được chọn.")]
        private CustomerDto? _selectedCustomer;
        [ObservableProperty]
        private ObservableCollection<CustomerDto> _customerSuggestions;

        [ObservableProperty]
        private bool _isAddContact;

        [ObservableProperty]
        private ObservableCollection<AddContactItemViewModel> _contactItems = new();

        [ObservableProperty]
        private int? _employeeId;
        [ObservableProperty]
        private string _employeeSearchKeyword = string.Empty;
        [ObservableProperty]
        private decimal? _expectedPrice;
        [ObservableProperty]
        private int _selectedProjectId;
        [ObservableProperty]
        private int _selectedProductId;
        [ObservableProperty]
        private int? _productQuantity;

        [ObservableProperty]
        private int _opportunityStatusId;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Tên cơ hội không được để trống.")]
        [MinLength(3, ErrorMessage = "Tên cơ hội phải có ít nhất 3 ký tự.")]
        private string _opportunityName;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Ngày bắt đầu không được để trống.")]
        private DateTime? _opportunityStartDate = DateTime.UtcNow;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Ngày kết thúc không được để trống.")]
        private DateTime? _opportunityEndDate = DateTime.UtcNow;

        [ObservableProperty]
        private bool _isValid = true;

        [ObservableProperty]
        private ObservableCollection<EmployeeDto> _employeeSuggestions;
        [ObservableProperty]
        private EmployeeDto? _selectedEmployee;

        [ObservableProperty]
        private ObservableCollection<CustomerTypeOption> _customerTypeOptions;
        [ObservableProperty]
        private ObservableCollection<SalutationOption> _salutationOptions;
        [ObservableProperty]
        private ObservableCollection<OpportunityStatusOption> _opportunityStatusOptions;
        [ObservableProperty]
        private ObservableCollection<ProjectDto> _projectOptions;
        [ObservableProperty]
        private ObservableCollection<ProductDto> _productOptions;
        [ObservableProperty]
        private ProductDto? _selectedProduct;

        [ObservableProperty]
        private ObservableCollection<ContactTypeOption> _contactTypeOptions;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng chọn loại liên hệ")]
        private int _contactTypeId;

        [ObservableProperty]
        private string? _validationMessage;

        public ConvertStageViewModel(
            ICustomerService customerService,
            IOpportunityService oppoturnityService,
            IProjectService projectService,
            IEmployeeService employeeService,
            IContactService contactService)
        {
            _customerService = customerService;
            _opportunityService = oppoturnityService;
            _projectService = projectService;
            _employeeService = employeeService;
            _contactService = contactService;

            CustomerTypeOptions = new();
            SalutationOptions = new();
            OpportunityStatusOptions = new();
            ProjectOptions = new();
            ProductOptions = new();
            CustomerSuggestions = new();
            EmployeeSuggestions = new();
            ContactItems = new();
            ContactTypeOptions = new();

            //Task.Run(async () => await LoadDataAsync());

            IsValid = true;
            IsAddContact = false;
        }

        public async Task LoadDataAsync()
        {
            await LoadSalutationsAsync();
            await LoadOpportunityStatusesAsync();
            await LoadProjectsAsync();
            await LoadCustomerTypesAsync();
            await SearchEmployeesAsync();
            await SearchCustomersAsync();
            await LoadContactTypesAsync();
            //await LoadProductsAsync();
        }

        public void SetLeadId(int leadId)
        {
            LeadId = leadId;
        }

        #region Private Methods
        private async Task SearchCustomersAsync()
        {
            //IsCustomerDropDownOpen = true;
            //if (string.IsNullOrWhiteSpace(CustomerSearchKeyword))
            //{
            //    IsCustomerDropDownOpen = false;
            //    SelectedCustomer = null;
            //    return;
            //}

            var request = new GetCustomerRequest
            {
                Keyword = CustomerSearchKeyword,
                PageNumber = 1,
                PageSize = 10
            };

            var pagedCustomer = await _customerService.GetAllCustomersAsync(request);

            var customers = pagedCustomer.Items.ToList();

            CustomerSuggestions.Clear();

            foreach (var cust in customers)
            {
                CustomerSuggestions.Add(cust);
            }
        }

        private async Task SearchEmployeesAsync()
        {
            //IsEmployeeDropDownOpen = true;
            //if (string.IsNullOrWhiteSpace(EmployeeSearchKeyword))
            //{
            //    IsEmployeeDropDownOpen = false;
            //    EmployeeSuggestions.Clear();
            //    return;
            //}

            var request = new GetEmployeeRequest
            {
                Keyword = EmployeeSearchKeyword,
                PageNumber = 1,
                PageSize = 10
            };

            var pagedEmployee = await _employeeService.GetAllEmployeesAsync(request);

            var employees = pagedEmployee.Items.ToList();

            EmployeeSuggestions.Clear();
            foreach (var emp in employees)
            {
                EmployeeSuggestions.Add(emp);
            }
        }

        private async Task AddAllContactAsync()
        {
            if (SelectedCustomer == null)
            {
                return;
            }

            var newContacts = new List<AddContactRequest>();
            foreach (var item in ContactItems)
            {
                var validationError = item.ValidateContactInput();
                if (validationError != null)
                {
                    ValidationMessage = validationError;
                    IsValid = false;
                    return;
                }

                var newContact = new AddContactRequest
                {
                    ContactName = item.Name,
                    ContactPhone = item.Phone,
                    ContactEmail = item.Email,
                    ContactAddress = item.Address,
                    ContactSalutationId = item.SalutationId,
                    ContactDescription = item.Description,
                    ContactRole = item.Role,
                    ContactTypeId = item.ContactTypeId
                };
                newContacts.Add(newContact);

            }
            if (newContacts.Count != 0)
            {
                await _customerService.AddContactRangeAsync(SelectedCustomer.Id, newContacts);

                ContactItems.Clear();

                IsAddContact = false;

                ValidationMessage = string.Empty;

                IsValid = true;

                MessageBox.Show("Thêm liên hệ thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool ValidateOpportunityInput()
        {
            var errors = new List<string>();
            if (SelectedCustomer == null)
            {
                errors.Add("Khách hàng phải được chọn.");
            }
            if (EmployeeId == 0)
            {
                errors.Add("Nhân viên phụ trách phải được chọn.");
            }
            if (SelectedProjectId == 0)
            {
                errors.Add("Dự án phải được chọn.");
            }
            if (SelectedProduct == null)
            {
                errors.Add("Sản phẩm phải được chọn.");
            }
            if (ProductQuantity <= 0)
            {
                errors.Add("Số lượng sản phẩm phải lớn hơn 0.");
            }
            if (ExpectedPrice <= 0)
            {
                errors.Add("Giá dự kiến phải lớn hơn 0.");
            }
            if (OpportunityStatusId == 0)
            {
                errors.Add("Trạng thái cơ hội phải được chọn.");
            }
            if (errors.Any())
            {
                ValidationMessage = string.Join("\n", errors);
                IsValid = false;
            }
            else
            {
                ValidationMessage = string.Empty;
                IsValid = true;
            }
            SaveOpportunityCommand.NotifyCanExecuteChanged();

            return IsValid;
        }

        private async Task AddOpportunityAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            if (SelectedCustomer == null)
            {
                return;
            }

            if (!ValidateOpportunityInput())
            {
                return;
            }

            var newOpportunity = new AddOpportunityRequest
            {
                OpportunityName = OpportunityName,
                OpportunityDescription = $"Cơ hội được tạo từ khách hàng {SelectedCustomer.Name}",
                OpportunityStatusId = OpportunityStatusId,
                CustomerId = SelectedCustomer.Id,
                EmployeeId = EmployeeId ?? 0,
                StartDate = OpportunityStartDate,
                EndDate = OpportunityEndDate,
                OpportunityItems = new List<AddOpportunityItemRequest>
                {
                    new AddOpportunityItemRequest
                    {
                        ProductId = SelectedProduct?.ProductId ?? 0,
                        Quantity = 1,
                        ExpectedPrice = ExpectedPrice ?? 0,
                        Price = SelectedProduct?.ProductPrice ?? 0
                    }
                }
            };

            await _opportunityService.AddOpportunityAsync(newOpportunity);

            MessageBox.Show("Chuyển đổi sang cơ hội thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task LoadCustomerTypesAsync()
        {
            var options = await _customerService.GetAllCustomerTypeAsync();

            CustomerTypeOptions = new(options);
        }

        private async Task LoadContactTypesAsync()
        {
            var options = await _contactService.GetContactTypeOptionsAsync();
            ContactTypeOptions = new(options);
        }

        private async Task LoadSalutationsAsync()
        {
            var options = await _customerService.GetAllSalutationsAsync();
            SalutationOptions = new(options);
        }

        private async Task LoadOpportunityStatusesAsync()
        {
            var options = await _opportunityService.GetAllOpportunityStatusesAsync();
            OpportunityStatusOptions = new(options);
        }

        private async Task LoadProjectsAsync()
        {
            var request = new GetProjectRequest
            {
                PageNumber = 1,
                PageSize = 1000
            };
            var projects = await _projectService.GetProjectsAsync(request);
            ProjectOptions = new(projects.Items);

        }

        private async Task LoadProductsAsync(int projectId)
        {
            var products = await _projectService.GetProductsByProjectIdAsync(projectId);
            if (!products.Any())
            {
                ProductOptions.Clear();
                MessageBox.Show("Dự án hiện tại đã hết sản phẩm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            ProductOptions = new(products);
        }

        #endregion

        #region Commands

        [RelayCommand]
        private void AddNewContact()
        {
            ContactItems.Add(new AddContactItemViewModel());
        }

        [RelayCommand]
        private void RemoveContactItem(AddContactItemViewModel item)
        {
            if (ContactItems.Contains(item))
            {
                ContactItems.Remove(item);
            }
        }

        [RelayCommand]
        private async Task SaveContactsAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            if (HasErrors)
            {
                MessageBox.Show("Vui lòng kiểm tra lỗi trước khi lưu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                IsValid = true;

                if (IsNewCustomer)
                {
                    var newCustomer = new AddCustomerRequest
                    {
                        Name = NewCustomerName,
                        CustomerTypeId = NewCustomerTypeId,
                        Phone = NewCustomerPhone,
                        IdentityCard = NewCustomerIdCard,
                        LeadId = LeadId // khách hàng tạo từ lead (nếu có)
                    };

                    var result = await _customerService.AddCustomerAsync(newCustomer);
                    if (!result.IsSuccess)
                    {
                        ValidationMessage = result.Error.Message;
                        IsValid = false;
                        return;
                    }

                    SelectedCustomer = result.Value;
                }

                if (SelectedCustomer == null)
                {
                    ValidationMessage = "Vui lòng chọn khách hàng trước khi thêm liên hệ.";
                    IsValid = false;
                    return;
                }

                if (IsExistingCustomer)
                {
                    // cập nhật leadId cho khách hàng đã chọn
                    var updateCustomerRequest = new UpdateCustomerRequest
                    {
                        Id = SelectedCustomer.Id,
                        Name = SelectedCustomer.Name,
                        PhoneNumber = SelectedCustomer.Phone,
                        LeadId = LeadId
                    };
                    var updateResult = await _customerService.UpdateCustomerAsync(updateCustomerRequest);
                    if (!updateResult.IsSuccess)
                    {
                        ValidationMessage = updateResult.Error.Message;
                        IsValid = false;
                        return;
                    }
                }

                if (ContactItems.Count == 0)
                {
                    ValidationMessage = "Vui lòng thêm ít nhất một liên hệ.";
                    IsValid = false;
                    return;
                }

                await AddAllContactAsync();
            }
            catch (Exception)
            {
                IsValid = false;
            }
        }

        [RelayCommand]
        private async Task SaveOpportunityAsync()
        {
            try
            {
                IsValid = true;

                if (IsNewCustomer)
                {
                    var newCustomer = new AddCustomerRequest
                    {
                        Name = NewCustomerName,
                        CustomerTypeId = NewCustomerTypeId,
                        Phone = NewCustomerPhone,
                        IdentityCard = NewCustomerIdCard,
                        LeadId = LeadId // khách hàng tạo từ lead (nếu có)
                    };

                    var result = await _customerService.AddCustomerAsync(newCustomer);
                    if (!result.IsSuccess)
                    {
                        ValidationMessage = result.Error.Message;
                        IsValid = false;
                        return;
                    }

                    SelectedCustomer = result.Value;

                    // set lại IsNewCustomer để tránh thêm lại
                    SelectedTypeAction = "Khách hàng cũ";
                }

                if (SelectedCustomer == null)
                {
                    ValidationMessage = "Vui lòng chọn khách hàng.";
                    IsValid = false;
                    return;
                }

                if (IsExistingCustomer)
                {
                    // cập nhật leadId cho khách hàng đã chọn
                    var updateCustomerRequest = new UpdateCustomerRequest
                    {
                        Id = SelectedCustomer.Id,
                        Name = SelectedCustomer.Name,
                        PhoneNumber = SelectedCustomer.Phone,
                        LeadId = LeadId
                    };
                    var updateResult = await _customerService.UpdateCustomerAsync(updateCustomerRequest);
                    if (!updateResult.IsSuccess)
                    {
                        ValidationMessage = updateResult.Error.Message;
                        IsValid = false;
                        return;
                    }
                }

                if (ContactItems.Count > 0)
                {
                    await AddAllContactAsync();
                }

                await AddOpportunityAsync();

                IsDialogVisible = false;

                MessageBox.Show("Chuyển đổi khách hàng thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                System.Windows.Application.Current.Windows
                    .OfType<Window>()
                    .SingleOrDefault(w => w.DataContext == this)?.Close();
            }
            catch (Exception ex)
            {
                IsValid = false;
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            IsDialogVisible = false;
            DialogCancelled?.Invoke();
        }

        #endregion

        #region Events
        public event Action? DialogCancelled;
        #endregion

        #region Property Changed Callbacks

        partial void OnSelectedProjectIdChanged(int value)
        {
            _ = LoadProductsAsync(value);
        }

        partial void OnEmployeeSearchKeywordChanged(string value)
        {
            if (_selectedEmployee?.Name != value)
            {
                SelectedEmployee = null;
                _ = SearchEmployeesAsync();
            }
        }

        partial void OnCustomerSearchKeywordChanged(string value)
        {
            if (_selectedCustomer?.Name != value)
            {
                SelectedCustomer = null;
                _ = SearchCustomersAsync();
            }
        }

        partial void OnSelectedCustomerChanged(CustomerDto? value)
        {
            if (value != null)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    CustomerSearchKeyword = value.Name;
                    IsCustomerDropDownOpen = false;
                }));
            }
        }

        partial void OnSelectedEmployeeChanged(EmployeeDto? value)
        {
            if (value != null)
            {
                EmployeeId = value.Id;
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    EmployeeSearchKeyword = value.Name;
                    IsEmployeeDropDownOpen = false;
                }));
            }
        }

        partial void OnSelectedProductChanged(ProductDto? value)
        {
            if (value != null)
            {
                ExpectedPrice = value.ProductPrice;
            }
        }

        #endregion
    }
}
