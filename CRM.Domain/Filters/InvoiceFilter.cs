using CRM.Domain.Filters.Base;

namespace CRM.Domain.Filters
{
    public class InvoiceFilter : FilterBase
    {
        public bool? IsPaid { get; set; }
    }
}
