using CRM.Domain.Filters;
using CRM.Domain.Models;
using CRM.Shared.Results;

namespace CRM.Domain.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<PagedResult<Payment>> GetPaymentsAsync(PaymentFilter filter);
        Task<Payment?> GetPaymentByIdAsync(int id);
        Task AssignInvoiceCodeToInstallmentScheduleAsync(int installmentScheduleId, string invoiceCode);
    }
}
