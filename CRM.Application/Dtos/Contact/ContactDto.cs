using System.ComponentModel;

namespace CRM.Application.Dtos.Contact
{
    public class ContactDto
    {
        [DisplayName("ID")]
        public int Id { get; set; }
        [DisplayName("Tên liên hệ")]
        public string Name { get; set; } = string.Empty;
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; } = string.Empty;
        [DisplayName("Email")]
        public string Email { get; set; } = string.Empty;
        [DisplayName("Địa chỉ")]
        public string Address { get; set; } = string.Empty;
        public int SalutationId { get; set; }
        [DisplayName("Xưng hô")]
        public string? Salutation { get; set; } = string.Empty;
        public int ContactTypeId { get; set; }
        [DisplayName("Loại liên hệ")]
        public string? ContactType { get; set; } = string.Empty;
        [DisplayName("Ngày tạo")]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Mô tả")]
        public string? Description { get; set; } = string.Empty;
        [DisplayName("Tên khách hàng")]
        public string? CustomerName { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
    }
}
