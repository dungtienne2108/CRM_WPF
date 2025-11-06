using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Payment;
using CRM.Application.Interfaces.Payment;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.PaymentManagement
{
    public partial class AddPaymentViewModel : ViewModelBase
    {
        private readonly IPaymentService _paymentService;

        [ObservableProperty]
        private int _invoiceId;
        [ObservableProperty]
        private int _paymentMethodId;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Số tiền không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0.")]
        private decimal _amount;
        [ObservableProperty]
        private int _customerId;
        [ObservableProperty]
        private string _customerName;
        [ObservableProperty]
        private string _description;
        [ObservableProperty]
        private DateTime _paymentDate = DateTime.UtcNow;

        [ObservableProperty]
        private string _invoiceSearchKeyword;
        [ObservableProperty]
        private bool _isInvoiceDropdownOpen;
        [ObservableProperty]
        private ObservableCollection<PaymentMethodOption> _paymentMethodOptions = new();
        [ObservableProperty]
        private PaymentMethodOption _selectedPaymentMethod;
        [ObservableProperty]
        private ObservableCollection<InvoiceDto> _invoices = new();
        [ObservableProperty]
        private InvoiceDto _selectedInvoice;

        public AddPaymentViewModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;

            ErrorsChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(CanSave));
            };
        }

        #region Public Methods
        public async Task LoadDataAsync()
        {
            await GetPaymentMethodOptionsAsync();
            await GetInvoicesAsync();
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
                return;
            }
            await CreatePaymentAsync();
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
        #endregion

        #region Private Methods
        private async Task CreatePaymentAsync()
        {
            try
            {
                var request = new CreatePaymentRequest
                {
                    InvoiceId = InvoiceId,
                    PaymentMethodId = PaymentMethodId,
                    Amount = Amount,
                    CustomerId = CustomerId,
                    Description = Description,
                    PaymentDate = PaymentDate
                };
                var result = await _paymentService.CreatePaymentAsync(request);
                if (result.IsSuccess)
                {
                    MessageBox.Show("Thêm thanh toán thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    // đóng cửa sổ
                    System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
                }
                else
                {
                    MessageBox.Show($"Thêm thanh toán thất bại. Lỗi: {result.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Thêm thanh toán thất bại. Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task GetInvoicesAsync()
        {
            try
            {
                IsInvoiceDropdownOpen = true;
                var request = new GetInvoiceRequest
                {
                    Keyword = InvoiceSearchKeyword,
                    PageNumber = 1,
                    PageSize = 1000,
                    IsPaid = false
                };

                var invoiceResult = await _paymentService.GetInvoicesAsync(request);

                Invoices.Clear();

                foreach (var invoice in invoiceResult.Items)
                {
                    Invoices.Add(invoice);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lấy hóa đơn thất bại. Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                // đóng cửa sổ
                System.Windows.Application.Current.Windows
                    .OfType<Window>()
                    .SingleOrDefault(w => w.DataContext == this)?
                    .Close();
            }
        }
        private async Task GetPaymentMethodOptionsAsync()
        {
            var paymentMethodResult = await _paymentService.GetPaymentMethodOptionsAsync();
            if (paymentMethodResult.IsSuccess)
            {
                PaymentMethodOptions = new ObservableCollection<PaymentMethodOption>(paymentMethodResult.Value);
            }
            else
            {
                MessageBox.Show($"Lấy phương thức thanh toán thất bại. Lỗi: {paymentMethodResult.Error}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                // đóng cửa sổ
                System.Windows.Application.Current.Windows
                    .OfType<Window>()
                    .SingleOrDefault(w => w.DataContext == this)?
                    .Close();
            }
        }
        #endregion

        #region Property changed
        partial void OnInvoiceSearchKeywordChanged(string value)
        {
            if (InvoiceSearchKeyword != value)
            {
                _ = GetInvoicesAsync();
            }
        }

        partial void OnSelectedInvoiceChanged(InvoiceDto value)
        {
            if (value != null)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    InvoiceSearchKeyword = value.Code;
                    InvoiceId = value.Id;
                    CustomerId = value.CustomerId;
                    CustomerName = value.CustomerName;
                    IsInvoiceDropdownOpen = false;
                }));
            }
        }

        partial void OnSelectedPaymentMethodChanged(PaymentMethodOption value)
        {
            if (value != null)
            {
                PaymentMethodId = value.Id;
            }
        }
        #endregion
    }
}
