using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Payment;
using CRM.Application.Interfaces.Contract;
using CRM.Application.Interfaces.Payment;
using CRM.UI.ViewModels.Base;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.PaymentManagement
{
    public partial class AddPaymentScheduleViewModel : ViewModelBase
    {
        private readonly IContractService _contractService;
        private readonly IPaymentService _paymentService;

        [ObservableProperty]
        private int _contractId;
        [ObservableProperty]
        private string _contractNumber = string.Empty;
        [ObservableProperty]
        private string _contractName = string.Empty;
        [ObservableProperty]
        private decimal _contractValue;
        [ObservableProperty]
        private decimal _contractPaidAmount;
        [ObservableProperty]
        private decimal _contractRemainingValue;
        [ObservableProperty]
        private int _employeeId;
        [ObservableProperty]
        private string _employeeName = string.Empty;

        // số tiền đã thanh tonas và còn lại
        [ObservableProperty]
        private decimal _paidAmount;
        [ObservableProperty]
        private decimal _remainingAmount;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng nhập tên kế hoạch thanh toán")]
        [MinLength(3, ErrorMessage = "Tên kế hoạch thanh toán phải nhiều hơn 3 kí tự")]
        private string _scheduleName = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        private decimal _amount;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0, 100, ErrorMessage = "Phần trăm giá trị hợp đồng phải từ 0 đến 100")]
        private decimal _valuePercentage;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng chọn ngày đến hạn")]
        private DateTime _dueDate = DateTime.UtcNow;

        [ObservableProperty]
        private bool _isLastSchedule;

        public AddPaymentScheduleViewModel(IContractService contractService, IPaymentService paymentService)
        {
            _contractService = contractService;
            _paymentService = paymentService;
        }

        #region Public Methods
        public async Task LoadDataAsync(int contractId)
        {
            ContractId = contractId;
            await GetContractAsync();
        }
        #endregion

        #region Commands

        public bool CanSave() => !HasAnyErrors;

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            if (ContractId <= 0)
            {
                MessageBox.Show("Hợp đồng không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (HasErrors)
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin nhập.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Amount > ContractValue)
            {
                MessageBox.Show("Số tiền không được nhiều hơn giá trị hợp đồng", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await CreatePaymentScheduleAsync();

            // Đóng cửa sổ hiện tại
            System.Windows.Application.Current.Windows
                .OfType<Window>()
                .SingleOrDefault(w => w.DataContext == this)?
                .Close();
        }

        [RelayCommand]
        private void Cancel()
        {
            // Đóng cửa sổ hiện tại
            System.Windows.Application.Current.Windows
                .OfType<Window>()
                .SingleOrDefault(w => w.DataContext == this)?
                .Close();
        }
        #endregion

        #region Private Methods
        private async Task GetContractAsync()
        {
            var contractResult = await _contractService.GetContractByIdAsync(ContractId);

            if (contractResult.IsSuccess)
            {
                var contract = contractResult.Value;

                ContractNumber = contract.Number;
                ContractName = contract.Name;
                ContractValue = contract.AmountAfterTax;
                EmployeeId = contract.EmployeeId;
                EmployeeName = contract.EmployeeName;
                ContractRemainingValue = contract.RemainingAmount;
                ContractPaidAmount = contract.PaidAmount;
            }
        }

        private async Task CreatePaymentScheduleAsync()
        {
            try
            {
                var createPaymentScheduleRequest = new CreatePaymentScheduleRequest
                {
                    ContractId = ContractId,
                    InstallmentName = ScheduleName,
                    Amount = Amount,
                    ContractValuePercentage = Amount * 100 / ContractValue,
                    DueDate = DueDate
                };

                var result = await _paymentService.CreatePaymentScheduleAsync(createPaymentScheduleRequest);

                if (result.IsSuccess)
                {
                    MessageBox.Show("Thêm kế hoạch thanh toán thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    // đóng cửa sổ hiện tại
                    System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
                    return;
                }
                else
                {
                    MessageBox.Show($"Thêm kế hoạch thanh toán thất bại: {result.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Property changed
        partial void OnAmountChanged(decimal value)
        {
            if (value > ContractRemainingValue)
            {
                Amount = 0;
                ValuePercentage = 0;
                MessageBox.Show("Giá tiền không được quá số tiền còn lại", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ValuePercentage = ContractValue > 0
                            ? Math.Round((value / ContractValue) * 100, 2)
                            : 0;

        }

        partial void OnValuePercentageChanged(decimal value)
        {
            var remainingContractValuePercentage = ContractValue > 0
                                                ? Math.Round((ContractRemainingValue / ContractValue) * 100, 2)
                                                : 0;
            if (value < 0 || value > remainingContractValuePercentage)
            {
                Amount = 0;
                ValuePercentage = 0;
                MessageBox.Show("Giá tiền không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


        }

        partial void OnIsLastScheduleChanged(bool value)
        {
            if (value)
            {
                Amount = ContractRemainingValue;
                ValuePercentage = ContractRemainingValue > 0
                                ? Math.Round((Amount / ContractValue) * 100, 2)
                                : 0;
            }
            else
            {
                Amount = 0;
                ValuePercentage = ContractRemainingValue > 0
                                ? Math.Round((Amount / ContractValue) * 100, 2)
                                : 0;
            }
        }
        #endregion
    }
}
