using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Payment;
using CRM.Application.Interfaces.Payment;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.PaymentManagement
{
    public partial class PaymentDetailViewModel : ViewModelBase
    {
        private readonly IPaymentService _paymentService;

        [ObservableProperty]
        private int _paymentId;
        [ObservableProperty]
        private string _paymentCode = string.Empty;
        [ObservableProperty]
        private int _customerId;
        [ObservableProperty]
        private string _customerName = string.Empty;
        [ObservableProperty]
        private int _invoiceId;
        [ObservableProperty]
        private string _invoiceCode = string.Empty;
        [ObservableProperty]
        private decimal _amount;
        [ObservableProperty]
        private DateTime _paymentDate;
        [ObservableProperty]
        private int _paymentMethodId;
        [ObservableProperty]
        private string _paymentMethodName = string.Empty;
        [ObservableProperty]
        private string _paymentDescription = string.Empty;

        [ObservableProperty]
        private ObservableCollection<PaymentMethodOption> _paymentMethodOptions = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeleteVisible))]
        private bool _isEditMode = false;

        public bool IsDeleteVisible => !IsEditMode;

        public PaymentDetailViewModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        #region Public methods
        public async Task LoadDataAsync(PaymentItemViewModel paymentItem)
        {
            if (paymentItem == null)
            {
                MessageBox.Show("Lỗi khi lấy thanh toán", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            PaymentId = paymentItem.Id;
            await GetPaymentAsync(paymentItem.Id);
            await GetPaymentMethodsAsync();
        }
        #endregion

        #region Commands
        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();
        }
        #endregion

        #region private methods
        private async Task GetPaymentAsync(int id)
        {
            try
            {
                var paymentRes = await _paymentService.GetPaymentByIdAsync(id);
                if (paymentRes.IsSuccess)
                {
                    var payment = paymentRes.Value;
                    PaymentCode = payment.Code;
                    CustomerId = payment.CustomerId;
                    CustomerName = payment.CustomerName;
                    InvoiceId = payment.InvoiceId;
                    InvoiceCode = payment.InvoiceName;
                    Amount = payment.Amount;
                    PaymentDate = payment.PaymentDate;
                    PaymentMethodId = payment.PaymentMethodId;
                    PaymentMethodName = payment.PaymentMethod;
                    PaymentDescription = payment.Description;
                }
                else
                {
                    MessageBox.Show("Lỗi khi lấy thông tin thanh toán", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch { }
        }

        private async Task GetPaymentMethodsAsync()
        {
            try
            {
                var paymentMethodRes = await _paymentService.GetPaymentMethodOptionsAsync();
                if (paymentMethodRes.IsSuccess)
                {
                    PaymentMethodOptions.Clear();

                    foreach (var method in paymentMethodRes.Value)
                    {
                        PaymentMethodOptions.Add(method);
                    }
                }
                else
                {
                    MessageBox.Show("Lỗi khi lấy phương thức thanh toán", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch { }
        }
        #endregion

        #region Property Changed
        //partial void OnIsEditModeChanged(bool value)
        //{
        //    if (value)
        //    {
        //        _ = GetPaymentMethodsAsync();
        //    }
        //}
        #endregion
    }
}
