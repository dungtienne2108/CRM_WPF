using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Employee;
using CRM.Application.Dtos.Lead;
using CRM.Application.Dtos.Project;
using CRM.Application.Interfaces.Employee;
using CRM.Application.Interfaces.Leads;
using CRM.Application.Interfaces.Project;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.LeadManagement
{
    public partial class AddLeadViewModel : ViewModelBase
    {
        private readonly ILeadService _leadService;
        private readonly IProjectService _projectService;
        private readonly IEmployeeService _employeeService;

        //[ObservableProperty]
        //private string? code;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        private string name = string.Empty;

        [ObservableProperty]
        private string? company;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        private string? phone;

        [ObservableProperty]
        private string? email;

        [ObservableProperty]
        private string employeeName = string.Empty;

        [ObservableProperty]
        private int employeeId;

        [ObservableProperty]
        public string employeeSearchKeyword = string.Empty;

        [ObservableProperty]
        private bool _isEmployeeDropDownOpen;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        private DateTime startDate = DateTime.UtcNow;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        private DateTime endDate = DateTime.UtcNow;

        [ObservableProperty]
        public ObservableCollection<EmployeeDto> employeeSuggestions;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Nhân sự phụ trách là bắt buộc")]
        private EmployeeDto? _selectedEmployee;

        [ObservableProperty]
        private string? address;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Nguồn khách hàng là bắt buộc")]
        private int sourceId;

        //[ObservableProperty]
        //private string potentialLevel = string.Empty;

        [ObservableProperty]
        private int potentialLevelId;

        //[ObservableProperty]
        //private string status = string.Empty;

        [ObservableProperty]
        private int statusId;

        [ObservableProperty]
        private string? validationMessage;

        [ObservableProperty]
        private bool isValid;

        [ObservableProperty]
        private int _projectId;
        [ObservableProperty]
        private int _productId;
        [ObservableProperty]
        private decimal _price;

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

        public List<LeadPotentialLevelDto> PotentialLevelOptions { get; private set; }

        public List<LeadStageDto> StatusOptions { get; private set; }

        public List<LeadSourceDto> SourceOptions { get; private set; }

        public AddLeadViewModel(ILeadService leadService, IEmployeeService employeeService, IProjectService projectService)
        {
            _leadService = leadService;
            _employeeService = employeeService;
            _projectService = projectService;
            //// Set default values
            //Task.Run(async () =>
            //{
            //    await LoadDataAsync();
            //});
            IsValid = true;

            EmployeeSuggestions = new();

            ErrorsChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(CanSave));
            };
        }


        public async Task LoadDataAsync()
        {
            await GetPotentialLevelAsync();
            await GetLeadStageAsync();
            await GetLeadSourceAsync();
            await GetProjectsAsync();
            await SearchEmployeesAsync();
        }

        #region Command
        [RelayCommand]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            if (SelectedEmployee == null) return;
            if (!ValidateInput())
                return;

            var newLeadResult = await _leadService.AddLeadAsync(new AddLeadRequest
            {
                Name = Name,
                Company = Company,
                Phone = Phone,
                Email = Email,
                Address = Address,
                SourceId = SourceId,
                PotentialLevelId = PotentialLevelId,
                StatusId = StatusId,
                EmployeeId = SelectedEmployee.Id,
                StartDate = StartDate,
                EndDate = EndDate,
                LeadItems = SelectedProduct != null ? new List<AddLeadItemRequest>
                {
                    new AddLeadItemRequest
                    {
                        ProductId = SelectedProduct.ProductId,
                    }
                } : null
            });

            if (!newLeadResult.IsSuccess)
            {
                ValidationMessage = newLeadResult.Error.Message;
                return;
            }

            LeadCreated?.Invoke(newLeadResult.Value);
        }

        public bool CanSave => !HasAnyErrors;

        [RelayCommand]
        private void Cancel()
        {
            DialogCancelled?.Invoke();
        }

        [RelayCommand]
        private async Task SearchProjectAsync()
        {
            await GetProjectsAsync();
        }

        #endregion

        #region Validation

        private bool ValidateInput()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Tên khách hàng là bắt buộc");

            //if (string.IsNullOrWhiteSpace(EmployeeName))
            //    errors.Add("Nhân sự phụ trách là bắt buộc");
            if (EmployeeId == 0)
                errors.Add("Nhân sự phụ trách không hợp lệ");

            if (!string.IsNullOrWhiteSpace(Email) && !IsValidEmail(Email))
                errors.Add("Định dạng email không hợp lệ");

            if (!string.IsNullOrWhiteSpace(Phone) && !IsValidPhone(Phone))
                errors.Add("Định dạng số điện thoại không hợp lệ");

            if (errors.Any())
            {
                ValidationMessage = string.Join("; ", errors);
                IsValid = false;
            }
            else
            {
                ValidationMessage = null;
                IsValid = true;
            }

            // Thông báo cho SaveCommand cập nhật CanExecute
            SaveCommand.NotifyCanExecuteChanged();

            return IsValid;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            var cleanPhone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            return cleanPhone.Length >= 10 && cleanPhone.All(char.IsDigit);
        }

        #endregion

        #region Private Methods
        private async Task GetProjectsAsync()
        {
            try
            {
                //IsProjectDropDownOpen = true;
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
                var result = await _projectService.GetUnsoldProductsByProjectIdAsync(projectId);

                if (!result.Any())
                {
                    ProductOptions.Clear();
                    MessageBox.Show("Dự án hiện tại đã hết sản phẩm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                foreach (var product in result)
                {
                    ProductOptions.Add(product);
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task GetPotentialLevelAsync()
        {
            var leadPotentialLevels = await _leadService.GetAllLeadPotentialLevelsAsync();

            PotentialLevelOptions = new(leadPotentialLevels);
        }

        private async Task GetLeadStageAsync()
        {
            var leadStages = await _leadService.GetAllLeadStagesAsync();

            StatusOptions = new(leadStages);
        }

        private async Task GetLeadSourceAsync()
        {
            var leadSources = await _leadService.GetAllLeadSourcesAsync();

            SourceOptions = new(leadSources);
        }

        private async Task SearchEmployeesAsync()
        {
            //IsEmployeeDropDownOpen = true;
            //if (string.IsNullOrEmpty(EmployeeSearchKeyword))
            //{
            //    IsEmployeeDropDownOpen = false;
            //    EmployeeSuggestions.Clear();
            //    return;
            //}

            var request = new GetEmployeeRequest
            {
                Keyword = EmployeeSearchKeyword,
                PageNumber = 1,
                PageSize = 1000
            };

            var pagedEmployee = await _employeeService.GetAllEmployeesAsync(request);

            var employees = pagedEmployee.Items.ToList();

            EmployeeSuggestions.Clear();
            foreach (var emp in employees)
            {
                EmployeeSuggestions.Add(emp);
            }
        }

        #endregion

        #region Public Methods

        public void LoadLead(LeadDto lead)
        {
            //Code = lead.Code;
            Name = lead.Name;
            Company = lead.Company;
            Phone = lead.Phone;
            Email = lead.Email;
            EmployeeName = lead.EmployeeName;
            Address = lead.Address;
            //PotentialLevel = lead.PotentialLevel;
            //Status = lead.Status;

            //ValidateInput();
        }

        #endregion

        #region Events

        public event Action<LeadDto>? LeadCreated;
        public event Action? DialogCancelled;

        #endregion

        #region Property Changed Callbacks

        partial void OnEmployeeSearchKeywordChanged(string value)
        {
            if (_selectedEmployee?.Name != value)
            {
                SelectedEmployee = null;
                _ = SearchEmployeesAsync();
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

        partial void OnNameChanged(string value) => ValidateInput();

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

        partial void OnSelectedProductChanged(ProductDto? value)
        {
            if (value != null)
            {
                ProductId = value.ProductId;
                Price = value.ProductPrice.HasValue ? value.ProductPrice.Value : 0;
            }
        }
        #endregion
    }
}
