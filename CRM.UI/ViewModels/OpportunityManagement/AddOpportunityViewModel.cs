using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Customer;
using CRM.Application.Dtos.Employee;
using CRM.Application.Dtos.Opportunity;
using CRM.Application.Dtos.Project;
using CRM.Application.Interfaces.Customers;
using CRM.Application.Interfaces.Employee;
using CRM.Application.Interfaces.Opportunity;
using CRM.Application.Interfaces.Project;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.OpportunityManagement
{
    public partial class AddOpportunityViewModel : ViewModelBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IProjectService _projectService;
        private readonly ICustomerService _customerService;
        private readonly IOpportunityService _opportunityService;

        [ObservableProperty]
        private int _employeeId;
        [ObservableProperty]
        private int _customerId;
        [ObservableProperty]
        private int _projectId;
        [ObservableProperty]
        private int _productId;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng nhập tên cơ hội")]
        [MinLength(3, ErrorMessage = "Tên cơ hội phải có ít nhất 3 ký tự")]
        private string _opportunityName = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        private DateTime _startDate = DateTime.UtcNow;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        private DateTime _endDate = DateTime.UtcNow.AddDays(30);
        [ObservableProperty]
        private decimal _price;

        [ObservableProperty]
        private ObservableCollection<EmployeeDto> _employeeSuggestions = new();
        [ObservableProperty]
        private EmployeeDto? _selectedEmployee;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchEmployeeCommand))]
        private string _employeeSearchKeyword = string.Empty;
        [ObservableProperty]
        private bool _isEmployeeDropDownOpen;

        [ObservableProperty]
        private ObservableCollection<CustomerDto> _customerSuggestions = new();
        [ObservableProperty]
        private CustomerDto? _selectedCustomer;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchCustomerCommand))]
        private string _customerSearchKeyword = string.Empty;
        [ObservableProperty]
        private bool _isCustomerDropDownOpen;


        [ObservableProperty]
        private ObservableCollection<ProjectDto> _projectSuggestions = new();
        [ObservableProperty]
        private ProjectDto? _selectedProject;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchProjectCommand))]
        private string _projectSearchKeyword = string.Empty;
        [ObservableProperty]
        private bool _isProjectDropDownOpen;

        [ObservableProperty]
        private ObservableCollection<ProductDto> _productOptions = new();
        [ObservableProperty]
        private ProductDto? _selectedProduct;

        public AddOpportunityViewModel(
            IEmployeeService employeeService,
            IProjectService projectService,
            ICustomerService customerService,
            IOpportunityService opportunityService)
        {
            _employeeService = employeeService;
            _projectService = projectService;
            _customerService = customerService;
            _opportunityService = opportunityService;


        }

        #region Public Methods
        public async Task LoadDataAsync()
        {
            await GetEmployeesAsync();
            await GetCustomersAsync();
            await GetProjectsAsync();
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

            await CreateOpportunityAsync();
        }

        [RelayCommand]
        private void Cancel()
        {
            // đóng cửa sổ
            System.Windows.Application.Current.Windows
                .OfType<Window>()
                .SingleOrDefault(w => w.DataContext == this)?
                .Close();
        }

        [RelayCommand]
        private async Task SearchEmployeeAsync()
        {
            await GetEmployeesAsync();
        }

        [RelayCommand]
        private async Task SearchCustomerAsync()
        {
            await GetCustomersAsync();
        }

        [RelayCommand]
        private async Task SearchProjectAsync()
        {
            await GetProjectsAsync();
        }
        #endregion

        #region Private Methods
        private async Task CreateOpportunityAsync()
        {
            try
            {
                var addOpportunityRequest = new AddOpportunityRequest
                {
                    OpportunityName = OpportunityName,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    CustomerId = CustomerId,
                    EmployeeId = EmployeeId,
                    OpportunityStatusId = 1,
                    OpportunityItems = new List<AddOpportunityItemRequest>
                    {
                        new AddOpportunityItemRequest
                        {
                            ProductId = ProductId,
                            Quantity = 1,
                            Price = Price,
                            ExpectedPrice = Price
                        }
                    }
                };

                var result = await _opportunityService.AddOpportunityAsync(addOpportunityRequest);
                if (result != null)
                {
                    MessageBox.Show("Tạo cơ hội thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // đóng cửa sổ
                    System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
                }
                else
                {
                    MessageBox.Show("Tạo cơ hội thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task GetEmployeesAsync()
        {
            try
            {
                IsEmployeeDropDownOpen = true;
                var getEmployeeRequest = new GetEmployeeRequest
                {
                    Keyword = EmployeeSearchKeyword,
                    PageNumber = 1,
                    PageSize = 1000
                };

                var employeePaged = await _employeeService.GetAllEmployeesAsync(getEmployeeRequest);

                EmployeeSuggestions.Clear();
                foreach (var employee in employeePaged.Items)
                {
                    EmployeeSuggestions.Add(employee);
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
                IsCustomerDropDownOpen = true;
                var getCustomerRequest = new GetCustomerRequest
                {
                    Keyword = CustomerSearchKeyword,
                    PageNumber = 1,
                    PageSize = 1000
                };
                var customerPaged = await _customerService.GetAllCustomersAsync(getCustomerRequest);
                CustomerSuggestions.Clear();
                foreach (var customer in customerPaged.Items)
                {
                    CustomerSuggestions.Add(customer);
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task GetProjectsAsync()
        {
            try
            {
                IsProjectDropDownOpen = true;
                var getProjectRequest = new GetProjectRequest
                {
                    Keyword = ProjectSearchKeyword,
                    PageNumber = 1,
                    PageSize = 1000
                };
                var projectPaged = await _projectService.GetProjectsAsync(getProjectRequest);
                ProjectSuggestions.Clear();
                foreach (var project in projectPaged.Items)
                {
                    ProjectSuggestions.Add(project);
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task GetProductsByProjectIdAsync(int projectId)
        {
            try
            {
                ProductOptions.Clear();
                var result = await _projectService.GetProductsByProjectIdAsync(projectId);
                foreach (var product in result)
                {
                    ProductOptions.Add(product);
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region property changed
        partial void OnSelectedProjectChanged(ProjectDto? value)
        {
            if (value != null)
            {
                _ = GetProductsByProjectIdAsync(value.ProjectId);
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ProjectId = value.ProjectId;
                    ProjectSearchKeyword = value.ProjectName;
                    IsProjectDropDownOpen = false;

                }));
            }
        }

        partial void OnProjectSearchKeywordChanged(string value)
        {
            if (SelectedProject != null && SelectedProject.ProjectName != value)
                _ = GetProjectsAsync();
        }

        partial void OnSelectedEmployeeChanged(EmployeeDto? value)
        {
            if (value != null)
            {
                //IsEmployeeDropDownOpen = false;
                EmployeeId = value.Id;
                EmployeeSearchKeyword = value.Name;
                IsEmployeeDropDownOpen = false;
            }
        }

        partial void OnEmployeeSearchKeywordChanged(string value)
        {
            //_ = SearchEmployeeCommand.ExecuteAsync(null);
            if (SelectedEmployee != null && SelectedEmployee.Name != value)
                _ = GetEmployeesAsync();
        }

        partial void OnSelectedProductChanged(ProductDto? value)
        {
            if (value != null)
            {
                ProductId = value.ProductId;
                Price = value.ProductPrice.HasValue ? value.ProductPrice.Value : 0;
            }
        }

        partial void OnSelectedCustomerChanged(CustomerDto? value)
        {
            if (value != null)
            {
                //IsCustomerDropDownOpen = false;
                CustomerId = value.Id;
                CustomerSearchKeyword = value.Name;
                IsCustomerDropDownOpen = false;
            }
        }

        partial void OnCustomerSearchKeywordChanged(string value)
        {
            //_ = SearchCustomerCommand.ExecuteAsync(null);
            if (SelectedCustomer != null && SelectedCustomer.Name != value)
                _ = GetCustomersAsync();
        }
        #endregion
    }
}
