using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Payment;
using CRM.Application.Interfaces.Payment;
using CRM.Domain.Models;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.PaymentManagement
{
    public partial class InvoiceDetailViewModel : ViewModelBase
    {
        private readonly IPaymentService _paymentService;

        [ObservableProperty]
        private int _invoiceId;
        [ObservableProperty]
        private string _invoiceName = string.Empty;
        [ObservableProperty]
        private string _invoiceCode = string.Empty;
        [ObservableProperty]
        private int _contractId;
        [ObservableProperty]
        private string _contractName = string.Empty;
        [ObservableProperty]
        private int _installmentScheduleId;
        [ObservableProperty]
        private string _installmentScheduleName = string.Empty;
        [ObservableProperty]
        private DateTime _dueDate;
        [ObservableProperty]
        private decimal _amount;
        [ObservableProperty]
        private string _status = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeleteVisible))]
        private bool _isEditMode = false;

        public bool IsDeleteVisible => !IsEditMode;

        [ObservableProperty]
        private InvoiceStatus _selectedStatusOption;

        [ObservableProperty]
        private ObservableCollection<InvoiceStatus> _statusOptions =
            new ObservableCollection<InvoiceStatus>(Enum.GetValues(typeof(InvoiceStatus)) as InvoiceStatus[]);


        public InvoiceDetailViewModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        #region Public methods
        public async Task LoadDataAsync(InvoiceItemViewModel invoiceItem)
        {
            if (invoiceItem == null)
            {
                MessageBox.Show("Lỗi khi lấy hóa đơn", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            InvoiceId = invoiceItem.Id;
            await GetInvoiceAsync(invoiceItem.Id);
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

            await UpdateInvoiceAsync();
        }
        #endregion

        #region Private methods
        private async Task UpdateInvoiceAsync()
        {
            try
            {
                var updateInvoiceReq = new UpdateInvoiceRequest
                {
                    Id = InvoiceId,
                    DueDate = DueDate,
                    Amount = Amount,
                    Status = SelectedStatusOption
                };

                var res = await _paymentService.UpdateInvoiceAsync(updateInvoiceReq);
                if (res.IsSuccess)
                {
                    MessageBox.Show("Cập nhật hóa đơn thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetInvoiceAsync(res.Value.Id);

                    IsEditMode = false;
                    System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
                }
                else
                {
                    MessageBox.Show("Cập nhật hóa đơn thất bại", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch { }
        }

        private async Task GetInvoiceAsync(int invoiceId)
        {
            try
            {
                var invoiceRes = await _paymentService.GetInvoiceByIdAsync(invoiceId);
                if (invoiceRes.IsSuccess)
                {
                    var invoice = invoiceRes.Value;
                    InvoiceCode = invoice.Code;
                    ContractId = invoice.ContractId;
                    ContractName = invoice.ContractName;
                    InstallmentScheduleId = invoice.InstallmentScheduleId;
                    InstallmentScheduleName = invoice.InstallmentScheduleName;
                    DueDate = invoice.DueDate;
                    Amount = invoice.Amount;
                    Status = invoice.Status switch
                    {
                        Domain.Models.InvoiceStatus.Pending => "Chưa thanh toán",
                        Domain.Models.InvoiceStatus.Paid => "Đã thanh toán",
                        Domain.Models.InvoiceStatus.Overdue => "Quá hạn",
                        Domain.Models.InvoiceStatus.Canceled => "Đã hủy",
                        _ => "Không xác định"
                    };
                }
                else
                {
                    MessageBox.Show("Lỗi khi lấy hóa đơn", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {

            }
        }
        #endregion
    }
}
