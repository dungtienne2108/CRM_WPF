using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Customer;
using CRM.Application.Dtos.Employee;
using CRM.Application.Interfaces.Customers;
using CRM.Application.Interfaces.Employee;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.CustomerManagement
{
    public partial class AddCustomerViewModel : ViewModelBase
    {
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Tên khách hàng không được để trống.")]
        [MinLength(3, ErrorMessage = "Tên khách hàng phải có ít nhất 3 ký tự.")]
        private string _customerName = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        [Required(ErrorMessage = "Email không được để trống.")]
        private string _customerEmail = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        private string _customerPhone = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Giới tính phải được chọn.")]
        private int _genderId;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "CMND/CCCD không được để trống.")]
        private string _customerIdentityCard = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Loại khách hàng phải được chọn.")]
        private int _customerTypeId;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Nhân viên phụ trách phải được chọn.")]
        private int _employeeId;
        [ObservableProperty]
        private string _customerAddress = string.Empty;
        [ObservableProperty]
        private string _customerDescription = string.Empty;

        [ObservableProperty]
        private ObservableCollection<CustomerTypeOption> _customerTypeOptions = new();
        [ObservableProperty]
        private ObservableCollection<GenderOption> _genderOptions = new();
        [ObservableProperty]
        private ObservableCollection<EmployeeDto> _employeeSuggestions = new();
        [ObservableProperty]
        private EmployeeDto? _selectedEmployee;
        [ObservableProperty]
        private string _employeeSearchKeyword = string.Empty;
        [ObservableProperty]
        private bool _isEmployeeDropDownOpen = false;

        public AddCustomerViewModel(ICustomerService customerService, IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _customerService = customerService;

            ErrorsChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(CanSave));
            };
        }

        #region Public Methods
        public async Task LoadDataAsync()
        {
            await GetGenderOptionsAsync();
            await GetCustomerTypeAsync();
            await GetEmployeeAsync();
        }
        #endregion

        #region Commands
        public bool CanSave => !HasErrors;

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            if (HasErrors)
            {
                MessageBox.Show("Vui lòng sửa các lỗi trước khi lưu.", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await CreateCustomerAsync();
        }

        [RelayCommand]
        private void Cancel()
        {
            System.Windows.Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this)?.Close();
        }
        #endregion

        #region Private Methods
        private async Task CreateCustomerAsync()
        {
            try
            {
                var newCustomer = new AddCustomerRequest
                {
                    Name = CustomerName,
                    Email = CustomerEmail,
                    Phone = CustomerPhone,
                    GenderId = GenderId,
                    IdentityCard = CustomerIdentityCard,
                    CustomerTypeId = CustomerTypeId,
                    EmployeeId = EmployeeId,
                    Address = CustomerAddress,
                    Description = CustomerDescription
                };
                var result = await _customerService.AddCustomerAsync(newCustomer);
                if (result.IsSuccess)
                {
                    MessageBox.Show("Khách hàng đã được tạo thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    Cancel();
                }
                else
                {
                    MessageBox.Show("Lỗi khi tạo khách hàng: " + result.Error.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task GetEmployeeAsync()
        {
            try
            {
                var getEmployeeRequest = new GetEmployeeRequest
                {
                    Keyword = EmployeeSearchKeyword,
                    PageNumber = 1,
                    PageSize = 1000
                };

                var result = await _employeeService.GetAllEmployeesAsync(getEmployeeRequest);

                EmployeeSuggestions.Clear();

                foreach (var employee in result.Items)
                {
                    EmployeeSuggestions.Add(employee);
                }
            }
            catch (Exception)

            {
                // Handle exceptions (e.g., log error, show message to user)
            }
        }

        private async Task GetGenderOptionsAsync()
        {
            try
            {
                var gendersResult = await _employeeService.GetAllGendersAsync();
                if (gendersResult.IsSuccess)
                {
                    GenderOptions.Clear();
                    foreach (var gender in gendersResult.Value)
                    {
                        GenderOptions.Add(gender);
                    }
                }
                else
                {
                    MessageBox.Show("Lỗi khi lấy danh sách giới tính: " + gendersResult.Error.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task GetCustomerTypeAsync()
        {
            try
            {
                var customerTypesResult = await _customerService.GetAllCustomerTypeAsync();
                if (customerTypesResult.Count > 0)
                {
                    CustomerTypeOptions.Clear();
                    foreach (var customerType in customerTypesResult)
                    {
                        CustomerTypeOptions.Add(customerType);
                    }
                }
                else
                {
                    MessageBox.Show("Lỗi khi lấy danh sách loại khách hàng", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        #endregion

        #region Property changed
        partial void OnEmployeeSearchKeywordChanged(string value)
        {
            if (SelectedEmployee != null && SelectedEmployee.Name != value)
            {
                _ = GetEmployeeAsync();
            }
        }

        partial void OnSelectedEmployeeChanged(EmployeeDto? value)
        {
            if (value != null)
            {
                EmployeeId = value.Id;
                EmployeeSearchKeyword = value.Name;
                IsEmployeeDropDownOpen = false;
            }
        }
        #endregion
    }
}
