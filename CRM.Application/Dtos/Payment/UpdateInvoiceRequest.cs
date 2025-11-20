using CRM.Domain.Models;

namespace CRM.Application.Dtos.Payment
{
    public class UpdateInvoiceRequest
    {
        public int Id { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public InvoiceStatus Status { get; set; }
    }
}
