using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Contract;
using CRM.Application.Dtos.Payment;
using CRM.Application.Interfaces.Contract;
using CRM.Application.Interfaces.Payment;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.PaymentManagement
{
    public partial class AddInvoiceViewModel : ViewModelBase
    {
        private readonly IContractService _contractService;
        private readonly IPaymentService _paymentService;

        [ObservableProperty]
        private int _employeeId;
        [ObservableProperty]
        private string _employeeName = string.Empty;
        [ObservableProperty]
        private int _contractId;
        [ObservableProperty]
        private string _contractName = string.Empty;
        [ObservableProperty]
        private int _paymentScheduleId;
        [ObservableProperty]
        private string _paymentScheduleName = string.Empty;
        [ObservableProperty]
        private int _customerId;
        [ObservableProperty]
        private string _customerName = string.Empty;
        //[ObservableProperty]
        //private string _invoiceNumber = string.Empty;
        [ObservableProperty]
        private DateTime _invoiceDate = DateTime.Now;
        [ObservableProperty]
        private DateTime _dueDate = DateTime.Now.AddDays(30);
        [ObservableProperty]
        private decimal _paymentScheduleAmount;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Số tiền là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        private decimal _amount;

        [ObservableProperty]
        private string _contractSearchKeyword = string.Empty;
        [ObservableProperty]
        private bool _isContractDropdownOpen;
        [ObservableProperty]
        private ObservableCollection<ContractDto> _contracts = new();
        [ObservableProperty]
        private ContractDto _selectedContract;
        [ObservableProperty]
        private ObservableCollection<PaymentScheduleDto> _paymentSchedules = new();
        [ObservableProperty]
        private PaymentScheduleDto _selectedPaymentSchedule;

        public AddInvoiceViewModel(IContractService contractService,
            IPaymentService paymentService)
        {
            _contractService = contractService;
            _paymentService = paymentService;

            ErrorsChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(CanSave));
            };
        }

        #region Public methods
        public async Task LoadDataAsync()
        {
            await GetContractsAsync();
        }
        #endregion

        #region commands

        public bool CanSave => !HasErrors;

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            if (HasErrors)
                return;

            await CreateInvoiceAsync();

            // đóng 
            System.Windows.Application.Current.Windows
                .OfType<Window>()
                .SingleOrDefault(w => w.DataContext == this)?
                .Close();
        }

        [RelayCommand]
        private void Cancel()
        {
            // đóng 
            System.Windows.Application.Current.Windows
                .OfType<Window>()
                .SingleOrDefault(w => w.DataContext == this)?
                .Close();
        }

        #endregion

        #region Private Methods
        private async Task CreateInvoiceAsync()
        {
            try
            {
                var createInvoiceRequest = new CreateInvoiceRequest
                {
                    ContractId = ContractId,
                    PaymentScheduleId = PaymentScheduleId,
                    InvoiceDate = InvoiceDate,
                    DueDate = DueDate,
                    Amount = Amount
                };

                var result = await _paymentService.CreateInvoiceAsync(createInvoiceRequest);

                if (result.IsSuccess)
                {
                    MessageBox.Show("Tạo hóa đơn thành công!");
                }
                else
                {
                    MessageBox.Show("Lỗi tạo hóa đơn: " + result.Error.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo hóa đơn: " + ex.Message);
            }
        }

        private async Task GetContractsAsync()
        {
            try
            {
                //IsContractDropdownOpen = true;
                var getContractRequest = new GetContractRequest
                {
                    Keyword = ContractSearchKeyword,
                    PageNumber = 1,
                    PageSize = 1000
                };

                var contractsResult = await _contractService.GetContractsAsync(getContractRequest);

                Contracts.Clear();

                foreach (var item in contractsResult.Items)
                {
                    Contracts.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải hợp đồng: " + ex.Message);
            }
        }

        private async Task GetPaymentSchedulesAsync(int contractId)
        {
            try
            {
                var paymentSchedulesResult = await _paymentService.GetPaymentSchedulesByContractIdAsync(contractId);
                PaymentSchedules.Clear();
                if (paymentSchedulesResult.IsSuccess)
                {
                    var paymentSchedules = paymentSchedulesResult.Value;
                    foreach (var item in paymentSchedules)
                    {
                        PaymentSchedules.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải kế hoạch thanh toán: " + ex.Message);
            }
        }
        #endregion

        #region Property changed
        partial void OnSelectedContractChanged(ContractDto value)
        {
            if (value != null)
            {
                _ = GetPaymentSchedulesAsync(value.Id);
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ContractId = value.Id;
                    ContractName = value.Name;
                    CustomerId = value.CustomerId;
                    EmployeeId = value.EmployeeId;
                    EmployeeName = value.EmployeeName;
                    CustomerName = value.CustomerName;
                    ContractSearchKeyword = value.Name;
                    IsContractDropdownOpen = false;
                }));
            }
        }

        partial void OnSelectedPaymentScheduleChanged(PaymentScheduleDto value)
        {
            if (value != null)
            {
                PaymentScheduleId = value.Id;
                PaymentScheduleName = value.InstallmentName;
                PaymentScheduleAmount = value.Amount;
            }
        }

        partial void OnContractSearchKeywordChanged(string value)
        {
            if (SelectedContract.Name != value)
            {
                _ = GetContractsAsync();
            }
        }
        #endregion
    }
}
