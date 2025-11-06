using System.ComponentModel;

namespace CRM.Application.Dtos.Customer
{
    public class CustomerDto
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Mã khách hàng")]
        public string? Code { get; set; }

        [DisplayName("Tên khách hàng")]
        public string Name { get; set; } = null!;

        [DisplayName("Số CCCD/CMND")]
        public string? CustomerIdentityCard { get; set; }

        public int TypeId { get; set; }

        [DisplayName("Loại khách hàng")]
        public string? TypeName { get; set; }

        [DisplayName("Số điện thoại")]
        public string? Phone { get; set; }

        [DisplayName("Email")]
        public string? Email { get; set; }

        [DisplayName("Giới tính")]
        public string? Gender { get; set; }

        [DisplayName("Địa chỉ")]
        public string? Address { get; set; }

        [DisplayName("Mô tả")]
        public string? Description { get; set; }

        public int? LeadId { get; set; }
        // [DisplayName("Trạng thái")]
        // public string Status { get; set; } = null!;

        // [DisplayName("Nhân viên phụ trách")]
        // public string EmployeeName { get; set; } = null!;
    }
}
