namespace CRM.Application.Dtos.Contact
{
    public class GetContactRequest
    {
        public string? Keyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
