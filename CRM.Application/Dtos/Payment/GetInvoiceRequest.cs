namespace CRM.Application.Dtos.Payment
{
    public class GetInvoiceRequest
    {
        public string Keyword { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsPaid { get; set; }
    }
}
