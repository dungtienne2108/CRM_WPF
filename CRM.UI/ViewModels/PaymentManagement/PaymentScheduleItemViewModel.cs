using CommunityToolkit.Mvvm.ComponentModel;
using CRM.Application.Dtos.Payment;
using CRM.UI.ViewModels.Base;

namespace CRM.UI.ViewModels.PaymentManagement
{
    public partial class PaymentScheduleItemViewModel : ViewModelBase
    {
        [ObservableProperty]
        private PaymentScheduleDto _paymentScheduleDTO;

        public PaymentScheduleItemViewModel(PaymentScheduleDto paymentScheduleDto, int index)
        {
            PaymentScheduleDTO = paymentScheduleDto;
            Index = index;
        }

        public int Index { get; }

        public int Id => PaymentScheduleDTO.Id;
        public int ContractId => PaymentScheduleDTO.ContractId;
        public string ContractName => PaymentScheduleDTO.ContractName;
        public string InstallmentName => PaymentScheduleDTO.InstallmentName;
        public decimal Amount => PaymentScheduleDTO.Amount;
        public decimal ContractValuePercentage => PaymentScheduleDTO.ContractValuePercentage;
        public DateTime? DueDate => PaymentScheduleDTO.DueDate;
        public string? Status => PaymentScheduleDTO.Status;
        public string InvoiceNumber => PaymentScheduleDTO.InvoiceNumber;
    }
}
