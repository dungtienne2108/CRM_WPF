namespace CRM.Application.Dtos.Customer
{
    public class UpdateCustomerRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? IdentityCard { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int? CustomerTypeId { get; set; }
        public string? Description { get; set; }
        public int? LeadId { get; set; } // khách hàng tạo từ lead (nếu có)
    }
}
