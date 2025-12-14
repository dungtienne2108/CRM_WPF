namespace CRM.Application.Dtos.Customer
{
    public class AddContactRequest
    {
        public string ContactName { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactAddress { get; set; }
        public int ContactSalutationId { get; set; }
        public string? ContactRole { get; set; }
        public string? ContactDescription { get; set; }
        public int ContactTypeId { get; set; }
    }
}
