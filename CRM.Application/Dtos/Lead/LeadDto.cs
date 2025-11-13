using CRM.Application.Dtos.Project;
using System.ComponentModel;

namespace CRM.Application.Dtos.Lead
{
    public sealed class LeadDto
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Mã tiềm năng")]
        public string? Code { get; set; }

        [DisplayName("Tên tiềm năng")]
        public string Name { get; set; } = null!;

        [DisplayName("Công ty")]
        public string? Company { get; set; }

        [DisplayName("Số điện thoại")]
        public string? Phone { get; set; }

        [DisplayName("Email")]
        public string? Email { get; set; }

        [DisplayName("Địa chỉ")]
        public string? Address { get; set; }

        [DisplayName("Mức độ tiềm năng")]
        public string PotentialLevel { get; set; } = string.Empty;

        public int PotentialLevelId { get; set; }

        [DisplayName("Nguồn tiềm năng")]
        public string Source { get; set; } = string.Empty;

        public int SourceId { get; set; }

        [DisplayName("Trạng thái")]
        public string Status { get; set; } = string.Empty;

        public int StatusId { get; set; }

        [DisplayName("Nhân viên phụ trách")]
        public string EmployeeName { get; set; } = null!;

        public int EmployeeId { get; set; }

        [DisplayName("Mô tả")]
        public string? Description { get; set; }

        [DisplayName("Ngày bắt đầu")]
        public DateTime? StartDate { get; set; }

        [DisplayName("Ngày kết thúc")]
        public DateTime? EndDate { get; set; }

        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
