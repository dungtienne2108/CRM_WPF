namespace CRM.Application.Dtos.Contact
{
    public class UpdateContactRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SalutationId { get; set; }
        public int ContactTypeId { get; set; }
        public int? CustomerId { get; set; }
    }
}
