namespace CRM.Application.Dtos.Customer
{
    public class AddCustomerRequest
    {
        public string Name { get; set; } = null!;
        public string? IdentityCard { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public int? GenderId { get; set; }
        public int? EmployeeId { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public int CustomerTypeId { get; set; }
        public int? LeadId { get; set; } // khách hàng tạo từ lead (nếu có)
    }
}
