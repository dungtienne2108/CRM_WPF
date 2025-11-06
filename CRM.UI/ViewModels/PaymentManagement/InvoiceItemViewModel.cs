using CommunityToolkit.Mvvm.ComponentModel;
using CRM.Application.Dtos.Payment;
using CRM.UI.ViewModels.Base;
using System.ComponentModel;

namespace CRM.UI.ViewModels.PaymentManagement
{
    public partial class InvoiceItemViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isSelected;
        [ObservableProperty]
        private InvoiceDto _invoiceDTO;

        public InvoiceItemViewModel(InvoiceDto invoiceDto, int index)
        {
            InvoiceDTO = invoiceDto;
            Index = index;
        }

        public int Index { get; set; }

        [DisplayName("ID")]
        public int Id => InvoiceDTO.Id;
        [DisplayName("Mã hóa đơn")]
        public string Code => InvoiceDTO.Code;
        [DisplayName("Số hóa đơn")]
        public string Number => InvoiceDTO.Number;
        [DisplayName("Tên hợp đồng")]
        public string ContractName => InvoiceDTO.ContractName;
        [DisplayName("Phương thức thanh toán")]
        public string PaymentMethod => InvoiceDTO.PaymentMethod;
        [DisplayName("Số tiền")]
        public decimal Amount => InvoiceDTO.Amount;
        [DisplayName("Ngày tạo")]
        public DateTime CreatedDate => InvoiceDTO.CreatedDate;
        [DisplayName("Ngày đến hạn")]
        public DateTime DueDate => InvoiceDTO.DueDate;
        [DisplayName("Trạng thái")]
        public string Status => InvoiceDTO.Status;
        [DisplayName("Thời hạn (ngày)")]
        public int DurationDays => (DueDate - CreatedDate).Days;

    }
}
