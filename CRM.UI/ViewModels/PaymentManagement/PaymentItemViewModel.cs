using CommunityToolkit.Mvvm.ComponentModel;
using CRM.Application.Dtos.Payment;
using CRM.UI.ViewModels.Base;
using System.ComponentModel;

namespace CRM.UI.ViewModels.PaymentManagement
{
    public partial class PaymentItemViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isSelected;
        [ObservableProperty]
        private PaymentDto _paymentDTO;

        public PaymentItemViewModel(PaymentDto paymentDto, int index)
        {
            PaymentDTO = paymentDto;
            Index = index;
        }

        public int Index { get; set; }

        [DisplayName("ID")]
        public int Id => PaymentDTO.Id;
        [DisplayName("Mã thanh toán")]
        public string Code => PaymentDTO.Code;
        [DisplayName("Số thanh toán")]
        public string Number => PaymentDTO.Number;
        [DisplayName("Tên khách hàng")]
        public string CustomerName => PaymentDTO.CustomerName;
        public int InvoiceId => PaymentDTO.InvoiceId;
        [DisplayName("Tên hóa đơn")]
        public string InvoiceName => PaymentDTO.InvoiceName;
        [DisplayName("Số tiền")]
        public decimal Amount => PaymentDTO.Amount;
        [DisplayName("Ngày thanh toán")]
        public DateTime PaymentDate => PaymentDTO.PaymentDate;
        [DisplayName("Phương thức thanh toán")]
        public string PaymentMethod => PaymentDTO.PaymentMethod;
    }
}
