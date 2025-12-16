using CRM.Domain.Filters;
using CRM.Domain.Models;
using CRM.Shared.Results;

namespace CRM.Domain.Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<PagedResult<Invoice?>> GetInvoicesAsync(InvoiceFilter filter);
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task UpdateStatusAsync(int id, InvoiceStatus status);
    }
}
