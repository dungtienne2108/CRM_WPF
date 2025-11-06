using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Contact;
using CRM.Application.Dtos.Deposit;
using CRM.Application.Dtos.Opportunity;
using CRM.Application.Interfaces.Contact;
using CRM.Application.Interfaces.Deposit;
using CRM.Application.Interfaces.Opportunity;
using CRM.Shared.Results;
using CRM.UI.ViewModels.Base;
using CRM.UI.ViewModels.OpportunityManagement;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.DepositManagement
{
    public partial class AddDepositViewModel : ViewModelBase
    {
        private readonly IDepositService _depositService;
        private readonly IContactService _contactService;
        private readonly IOpportunityService _opportunityService;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng nhập tên đặt cọc")]
        [MinLength(3, ErrorMessage = "Tên đặt cọc phải có ít nhất 3 ký tự")]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string _depositName = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        private decimal _amount;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        private DateTime _startDate = DateTime.UtcNow;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        private DateTime _endDate = DateTime.UtcNow;
        [ObservableProperty]
        private string? _description = string.Empty;

        [ObservableProperty]
        private int _customerId;
        [ObservableProperty]
        private string _customerName = string.Empty;
        [ObservableProperty]
        private int _employeeId;
        [ObservableProperty]
        private string _employeeName = string.Empty;
        [ObservableProperty]
        private int _opportunityId;
        [ObservableProperty]
        private string _opportunityName = string.Empty;
        [ObservableProperty]
        private int? _contactId;
        [ObservableProperty]
        private string? _contactName = string.Empty;
        [ObservableProperty]
        private string? _contactEmail = string.Empty;
        [ObservableProperty]
        private string? _contactPhone = string.Empty;
        [ObservableProperty]
        private string? _contactDescription = string.Empty;
        [ObservableProperty]
        private int _productId;
        [ObservableProperty]
        private string _productName = string.Empty;
        [ObservableProperty]
        private decimal _productPrice;
        [ObservableProperty]
        private int _productQuantity;

        [ObservableProperty]
        private ObservableCollection<ContactDto> _customerContacts = new();
        [ObservableProperty]
        private ContactDto _selectedContact;

        [ObservableProperty]
        private ObservableCollection<OpportunityItemDto> _opportunityItems = new();
        [ObservableProperty]
        private OpportunityItemDto _selectedOpportunityItem;

        public AddDepositViewModel(IDepositService depositService, IContactService contactService, IOpportunityService opportunityService)
        {
            _depositService = depositService;
            _contactService = contactService;
            _opportunityService = opportunityService;
        }

        #region Public Methods
        public async Task InitializeAsync(OpportunityItemViewModel opportunityItem)
        {
            if (opportunityItem == null)
                return;

            OpportunityId = opportunityItem.OpportunityId;
            OpportunityName = opportunityItem.OpportunityName;
            CustomerId = opportunityItem.Customer.Id;
            CustomerName = opportunityItem.Customer.Name;
            EmployeeId = opportunityItem.Employee.Id;
            EmployeeName = opportunityItem.Employee.Name;

            await GetContactsByCustomerId(CustomerId);
            await GetOpportunityItems(OpportunityId);
        }
        #endregion

        #region Commands

        public bool CanSave => !HasErrors && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();
            IsBusy = true;
            try
            {
                var result = await CreateDepositAsync();
                if (result.IsSuccess)
                {
                    MessageBox.Show("Tạo đặt cọc thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    // đóng cửa sổ hiện tại
                    System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .FirstOrDefault(w => w.DataContext == this)?
                        .Close();
                    return;
                }
                else
                {
                    SetError(result.Error);
                    MessageBox.Show($"Tạo đặt cọc thất bại: {result.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        #region Private Methods
        private async Task<Result> CreateDepositAsync()
        {
            try
            {
                var createDepositRequest = new CreateDepositRequest
                {
                    DepositName = DepositName,
                    DepositCosts = Amount,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Description = Description,
                    CustomerId = CustomerId,
                    EmployeeId = EmployeeId,
                    OpportunityId = OpportunityId,
                    ContactId = ContactId,
                    ProductId = ProductId
                };

                var result = await _depositService.CreateDepositAsync(createDepositRequest);

                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure(new("CREATE_DEPOSIT_FAILED", "Đã xảy ra lỗi khi tạo đặt cọc: " + ex.Message));
            }
        }

        private async Task GetContactsByCustomerId(int customerId)
        {
            try
            {
                var contacts = await _contactService.GetContactsByCustomerIdAsync(customerId);
                CustomerContacts.Clear();

                foreach (var contact in contacts)
                {
                    CustomerContacts.Add(contact);
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task GetOpportunityItems(int opportunityId)
        {
            try
            {
                var opportunityResult = await _opportunityService.GetOpportunityByIdAsync(opportunityId);

                OpportunityItems.Clear();

                if (opportunityResult.IsSuccess)
                {
                    var opportunity = opportunityResult.Value;
                    foreach (var item in opportunity.OpportunityItems)
                    {
                        OpportunityItems.Add(item);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region Property Changed Handlers
        partial void OnSelectedContactChanged(ContactDto value)
        {
            if (value != null)
            {
                ContactName = value.Name;
                ContactEmail = value.Email;
                ContactPhone = value.Phone;
                ContactId = value.Id;
                ContactDescription = value.Description ?? "";
            }
            else
            {
                ContactName = string.Empty;
                ContactEmail = string.Empty;
                ContactPhone = string.Empty;
                ContactDescription = string.Empty;
            }
        }

        partial void OnSelectedOpportunityItemChanged(OpportunityItemDto value)
        {
            if (value != null)
            {
                ProductId = value.ProductId;
                ProductName = value.ProductName;
                ProductPrice = value.Price;
                ProductQuantity = value.Quantity;
            }
            else
            {
                ProductId = 0;
                ProductName = string.Empty;
                ProductPrice = 0;
                ProductQuantity = 0;
            }
        }
        #endregion
    }
}
