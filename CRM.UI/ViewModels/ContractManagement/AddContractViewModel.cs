using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Contract;
using CRM.Application.Dtos.Deposit;
using CRM.Application.Interfaces.Contract;
using CRM.Application.Interfaces.Deposit;
using CRM.Application.Interfaces.Project;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.ContractManagement
{
    public partial class AddContractViewModel : ViewModelBase
    {
        private readonly IContractService _contractService;
        private readonly IDepositService _depositService;
        private readonly IProjectService _projectService;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Tên hợp đồng không được để trống")]
        [MinLength(3, ErrorMessage = "Tên hợp đồng phải có ít nhất 3 ký tự")]
        private string _contractName;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Số hợp đồng không được để trống")]
        [MinLength(3, ErrorMessage = "Số hợp đồng phải có ít nhất 3 ký tự")]
        private string _contractNumber;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Loại hợp đồng không được để trống")]
        private int _contractTypeId;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Giai đoạn hợp đồng không được để trống")]
        private int _contractStageId;
        [ObservableProperty]
        private int _customerId;
        [ObservableProperty]
        private string _customerName;
        [ObservableProperty]
        private string _seller = "Công ty FLC";
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Số tiền trước thuế không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền trước thuế phải lớn hơn hoặc bằng 0")]
        private decimal _amountBeforeTax;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Số tiền sau thuế không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền sau thuế phải lớn hơn hoặc bằng 0")]
        private decimal _amountAfterTax;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Thuế không được để trống")]
        [Range(0, 100, ErrorMessage = "Thuế phải từ 0 đến 100")]
        private decimal _tax;
        [ObservableProperty]
        private decimal _taxAmount;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Số tiền thực thu không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền thực thu phải lớn hơn hoặc bằng 0")]
        private decimal _amount;

        //public decimal AmountAfterTax => AmountBeforeTax + (AmountBeforeTax * Tax / 100);
        //public decimal Amount => AmountAfterTax - SelectedDeposit.Amount;

        [ObservableProperty]
        private DateTime _startDate = DateTime.UtcNow;
        [ObservableProperty]
        private DateTime _endDate = DateTime.UtcNow;

        [ObservableProperty]
        private int _depositId;
        [ObservableProperty]
        private decimal _depositAmount;

        [ObservableProperty]
        private int _projectId;
        [ObservableProperty]
        private string _projectName;
        [ObservableProperty]
        private int _productId;
        [ObservableProperty]
        private string _productName;
        [ObservableProperty]
        private decimal _productPrice;

        [ObservableProperty]
        private int _employeeId;

        [ObservableProperty]
        private ObservableCollection<ContractTypeOption> _contractTypeOptions = new();
        [ObservableProperty]
        private ObservableCollection<ContractStageOption> _contractStageOptions = new();
        [ObservableProperty]
        private ObservableCollection<DepositDto> _depositOptions = new();
        [ObservableProperty]
        private DepositDto _selectedDeposit;
        [ObservableProperty]
        private string _depositSearchKeyword;
        [ObservableProperty]
        private bool _isDepositDropdownOpen;

        public AddContractViewModel(IContractService contractService, IDepositService depositService, IProjectService projectService)
        {
            _contractService = contractService;
            _depositService = depositService;
            _projectService = projectService;
        }

        #region Public Methods
        public async Task LoadDataAsync()
        {
            await GetDepositsAsync();
            await GetContractTypeOptionsAsync();
            await GetContractStageOptionsAsync();
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();
            await AddContractAsync();
        }
        #endregion

        #region Private Methods
        private async Task AddContractAsync()
        {
            try
            {
                if (AmountBeforeTax < ProductPrice)
                {
                    MessageBox.Show($"Số tiền trước thuế không được nhỏ hơn giá sản phẩm ({ProductPrice:N0} VND).", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var createContractRequest = new CreateContractRequest
                {
                    ContractName = ContractName,
                    ContractNumber = ContractNumber,
                    ContractStageId = ContractStageId,
                    ContractTypeId = ContractTypeId,
                    CustomerId = CustomerId,
                    Seller = Seller,
                    AmountBeforeTax = AmountBeforeTax,
                    Tax = Tax,
                    AmountAfterTax = AmountAfterTax,
                    Amount = AmountAfterTax,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    DepositId = DepositId,
                    EmployeeId = EmployeeId,
                    ProductId = ProductId
                };

                var res = await _contractService.CreateContractAsync(createContractRequest);

                if (res.IsSuccess)
                {
                    MessageBox.Show("Thêm hợp đồng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    // đóng cửa sổ hiện tại
                    System.Windows.Application.Current.Windows
                            .OfType<Window>()
                            .SingleOrDefault(w => w.DataContext == this)?
                            .Close();
                    return;
                }
                else
                {
                    MessageBox.Show($"Lỗi : {res.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi : {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async Task GetDepositsAsync()
        {
            IsDepositDropdownOpen = true;
            try
            {
                var deposits = await _depositService.GetDepositsAsync(new GetDepositRequest
                {
                    Keyword = DepositSearchKeyword,
                    PageNumber = 1,
                    PageSize = 1000,
                    IsCreatedContract = false
                });

                DepositOptions.Clear();

                foreach (var item in deposits.Items)
                {
                    DepositOptions.Add(item);
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task GetContractStageOptionsAsync()
        {
            try
            {
                var result = await _contractService.GetContractStagesAsync();
                if (result.IsSuccess && result.Value != null)
                {
                    ContractStageOptions = new ObservableCollection<ContractStageOption>(result.Value);
                }

            }
            catch (Exception)
            {
            }
        }
        private async Task GetContractTypeOptionsAsync()
        {
            try
            {
                var result = await _contractService.GetContractTypesAsync();
                if (result.IsSuccess && result.Value != null)
                {
                    ContractTypeOptions = new ObservableCollection<ContractTypeOption>(result.Value);
                }

            }
            catch (Exception)
            { }
        }
        #endregion

        #region proerty changed
        partial void OnSelectedDepositChanged(DepositDto value)
        {
            if (value != null)
            {
                DepositId = value.Id;
                EmployeeId = value.EmployeeId;
                CustomerId = value.CustomerId;
                CustomerName = value.CustomerName;
                AmountBeforeTax = value.ProductPrice;
                ProjectId = value.ProductId;
                ProductId = value.ProductId;
                ProductName = value.ProductName;
                ProjectName = value.ProjectName;
                DepositAmount = value.Amount;
                ProductPrice = value.ProductPrice;
                IsDepositDropdownOpen = false;
                DepositSearchKeyword = value.Name;
            }
        }

        partial void OnAmountBeforeTaxChanged(decimal value)
        {
            if (AmountBeforeTax < ProductPrice)
            {
                MessageBox.Show($"Số tiền trước thuế không được nhỏ hơn giá sản phẩm ({ProductPrice:N0} VND).", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                AmountBeforeTax = ProductPrice;
                return;
            }
            else
            {
                ClearErrors(nameof(AmountBeforeTax));
            }

            AmountAfterTax = AmountBeforeTax + (AmountBeforeTax * Tax / 100);
            TaxAmount = AmountBeforeTax * Tax / 100;
            Amount = AmountAfterTax - DepositAmount;
        }

        partial void OnTaxChanged(decimal value)
        {
            AmountAfterTax = AmountBeforeTax + (AmountBeforeTax * Tax / 100);
            TaxAmount = AmountBeforeTax * Tax / 100;
            Amount = AmountAfterTax - DepositAmount;
        }

        partial void OnDepositSearchKeywordChanged(string value)
        {
            if (SelectedDeposit != null && SelectedDeposit.Name != value)
            {
                _ = GetDepositsAsync();
            }
        }
        #endregion
    }
}
