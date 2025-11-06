using System.ComponentModel;

namespace CRM.Application.Dtos.Employee
{
    public sealed class EmployeeDto
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Mã nhân viên")]
        public string Code { get; set; } = null!;

        [DisplayName("Tên nhân viên")]
        public string Name { get; set; } = null!;

        [DisplayName("Số CCCD/CMND")]
        public string IdentityCard { get; set; } = null!;

        [DisplayName("Email")]
        public string Email { get; set; } = null!;

        [DisplayName("Số điện thoại")]
        public string PhoneNumber { get; set; } = null!;

        public int GenderId { get; set; }

        [DisplayName("Giới tính")]
        public string GenderName { get; set; } = null!;

        [DisplayName("Địa chỉ")]
        public string? Address { get; set; }

        public int EmployeeTypeId { get; set; }

        [DisplayName("Loại nhân viên")]
        public string EmployeeTypeName { get; set; } = null!;

        [DisplayName("Ngày sinh")]
        public DateTime? Birthday { get; set; }

        [DisplayName("Mô tả")]
        public string? Description { get; set; }
    }
}
