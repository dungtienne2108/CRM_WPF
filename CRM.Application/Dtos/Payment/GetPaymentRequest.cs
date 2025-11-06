namespace CRM.Application.Dtos.Payment
{
    public class GetPaymentRequest
    {
        public string? Keyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
