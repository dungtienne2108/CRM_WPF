using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Contract;
using CRM.Application.Dtos.Customer;
using CRM.Application.Dtos.Deposit;
using CRM.Application.Dtos.Lead;
using CRM.Application.Dtos.Opportunity;
using CRM.Application.Interfaces.Contract;
using CRM.Application.Interfaces.Customers;
using CRM.Application.Interfaces.Deposit;
using CRM.Application.Interfaces.Leads;
using CRM.Application.Interfaces.Opportunity;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.CustomerManagement
{
    public partial class CustomerDetailViewModel : ViewModelBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILeadService _leadService;
        private readonly IOpportunityService _opportunityService;
        private readonly IContractService _contractService;
        private readonly IDepositService _depositService;

        [ObservableProperty]
        private CustomerItemViewModel _customer;

        [ObservableProperty]
        private int _customerId;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Tên khách hàng không được để trống.")]
        [MinLength(3, ErrorMessage = "Tên khách hàng không được bé hơn 3 ký tự.")]
        private string _customerName;
        [ObservableProperty]
        private string _customerCode;
        [ObservableProperty]
        private string _customerIdentityCard;
        [ObservableProperty]
        private int _customerTypeId;
        [ObservableProperty]
        private string _customerTypeName;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        private string _phone;
        [ObservableProperty]
        private string _email;
        [ObservableProperty]
        private string _address;
        [ObservableProperty]
        private string _businessField;
        [ObservableProperty]
        private string _description;
        [ObservableProperty]
        private int? _leadId;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeleteVisible))]
        private bool _isEditMode = false;

        public bool IsDeleteVisible => !IsEditMode;

        [ObservableProperty]
        private ObservableCollection<CustomerTypeOption> _customerTypes;

        [ObservableProperty]
        private ObservableCollection<LeadDto> _leads = new();
        [ObservableProperty]
        private ObservableCollection<OpportunityDto> _opportunities = new();
        [ObservableProperty]
        private ObservableCollection<ContractDto> _contracts = new();
        [ObservableProperty]
        private ObservableCollection<DepositDto> _deposits = new();

        public CustomerDetailViewModel(
            ICustomerService customerService,
            ILeadService leadService,
            IOpportunityService opportunityService,
            IContractService contractService,
            IDepositService depositService)
        {
            _customerService = customerService;
            _leadService = leadService;
            _opportunityService = opportunityService;
            _contractService = contractService;
            _depositService = depositService;
        }

        public async Task LoadDataAsync(CustomerItemViewModel customer)
        {
            if (customer == null)
            {
                MessageBox.Show("Khách hàng không tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Customer = customer;
            CustomerId = customer.Id;
            await LoadCustomerAsync(customer.Id);
            await LoadCustomerTypesAsync();

            // đi kèm
            await GetLeadsAsync();
            await GetContractsAsync();
            await GetOpportunitiesAsync();
            await GetDepositsAsync();
        }

        #region Private Methods
        private async Task LoadCustomerAsync(int customerid)
        {
            var customerResult = await _customerService.GetCustomerById(customerid);
            if (customerResult.IsSuccess)
            {
                var customer = customerResult.Value;
                Customer = new CustomerItemViewModel(customerResult.Value, Customer.Index);
                CustomerId = customer.Id;
                CustomerName = customer.Name;
                CustomerCode = customer.Code;
                CustomerIdentityCard = customer.CustomerIdentityCard;
                Phone = customer.Phone;
                Email = customer.Email;
                Address = customer.Address;
                Description = customer.Description;
                CustomerTypeId = customer.TypeId;
                CustomerTypeName = customer.TypeName;
                LeadId = customer.LeadId;
            }
        }

        private async Task LoadCustomerTypesAsync()
        {
            var customerTypeResult = await _customerService.GetAllCustomerTypeAsync();
            if (customerTypeResult.Count > 0)
            {
                CustomerTypes = new ObservableCollection<CustomerTypeOption>(customerTypeResult);
            }
        }

        private async Task GetLeadsAsync()
        {
            try
            {
                Leads.Clear();
                if (LeadId.HasValue)
                {
                    var leadResult = await _leadService.GetLeadByIdAsync(LeadId.Value);

                    if (leadResult.IsSuccess)
                    {
                        var lead = leadResult.Value;
                        Leads.Add(lead);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task GetOpportunitiesAsync()
        {
            try
            {
                Opportunities.Clear();

                var opportunitiesResult = await _opportunityService.GetOpportunitiesByCustomerIdAsync(CustomerId);

                if (opportunitiesResult.IsSuccess)
                {
                    foreach (var item in opportunitiesResult.Value)
                    {
                        Opportunities.Add(item);
                    }
                }
            }
            catch (Exception) { }
        }

        private async Task GetContractsAsync()
        {
            try
            {
                Contracts.Clear();

                var contractsResult = await _contractService.GetContractsByCustomerIdAsync(CustomerId);

                if (contractsResult.IsSuccess)
                {
                    foreach (var item in contractsResult.Value)
                    {
                        Contracts.Add(item);
                    }
                }
            }
            catch (Exception) { }
        }

        private async Task GetDepositsAsync()
        {
            try
            {
                Deposits.Clear();
                var depositsResult = await _depositService.GetDepositsByCustomerIdAsync(CustomerId);
                if (depositsResult.IsSuccess)
                {
                    foreach (var item in depositsResult.Value)
                    {
                        Deposits.Add(item);
                    }
                }
            }
            catch (Exception) { }
        }

        #endregion
        #region Commands
        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }

        [RelayCommand]
        private async Task UpdateCustomerAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            if (HasAnyErrors)
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin khách hàng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var updateRequest = new UpdateCustomerRequest
                {
                    Id = CustomerId,
                    Name = CustomerName,
                    IdentityCard = CustomerIdentityCard,
                    CustomerTypeId = CustomerTypeId,
                    PhoneNumber = Phone,
                    Email = Email,
                    Address = Address,
                    Description = Description
                };

                var updateResult = await _customerService.UpdateCustomerAsync(updateRequest);

                if (updateResult.IsSuccess)
                {
                    Customer = new CustomerItemViewModel(updateResult.Value, Customer.Index);
                    MessageBox.Show("Cập nhật thông tin khách hàng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadCustomerAsync(updateResult.Value.Id);
                    IsEditMode = false;

                    System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
                }
                else
                {
                    SetError(updateResult.Error);
                    MessageBox.Show($"Cập nhật thông tin khách hàng thất bại. Lỗi: {updateResult.Error}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
                MessageBox.Show($"Cập nhật thông tin khách hàng thất bại. Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            try
            {
                var res = MessageBox.Show("Bạn có chắc chắn xóa khách hàng?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res == MessageBoxResult.Yes)
                {
                    var deleteResult = await _customerService.DeleteCustomerAsync(CustomerId);

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
    }
}
