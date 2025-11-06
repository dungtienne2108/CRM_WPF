using CRM.Application.Dtos.Payment;
using CRM.Shared.Results;

namespace CRM.Application.Interfaces.Payment
{
    public interface IPaymentService
    {
        Task<Result<int>> CreatePaymentAsync(CreatePaymentRequest request);
        Task<Result<IEnumerable<PaymentMethodOption>>> GetPaymentMethodOptionsAsync();
        Task<PagedResult<InvoiceDto>> GetInvoicesAsync(GetInvoiceRequest request);
        Task<Result<InvoiceDto>> GetInvoiceByIdAsync(int invoiceId);
        Task<Result<InvoiceDto>> UpdateInvoiceAsync(UpdateInvoiceRequest invoiceDto);
        Task<Result<int>> CreateInvoiceAsync(CreateInvoiceRequest request);
        Task<PagedResult<PaymentDto>> GetPaymentsAsync(GetPaymentRequest request);
        Task<Result<PaymentDto>> GetPaymentByIdAsync(int paymentId);
        Task<Result<IEnumerable<PaymentScheduleDto>>> GetPaymentSchedulesByContractIdAsync(int contractId);
        Task<Result<int>> CreatePaymentScheduleAsync(CreatePaymentScheduleRequest request);
        Task<Result> DeletePaymentAsync(int paymentId);
    }
}
