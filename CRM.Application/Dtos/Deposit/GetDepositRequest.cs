namespace CRM.Application.Dtos.Deposit
{
    public class GetDepositRequest
    {
        public string? Keyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsCreatedContract { get; set; }
    }
}
