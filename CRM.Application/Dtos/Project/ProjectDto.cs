using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CRM.Application.Dtos.Project
{
    public sealed class ProjectDto
    {
        [DisplayName("ID")]
        public int ProjectId { get; set; }

        [DisplayName("Mã code")]
        public string? ProjectCode { get; set; }

        [DisplayName("Tên dự án")]
        public string ProjectName { get; set; } = null!;

        [DisplayName("Địa chỉ dự án")]
        public string? ProjectAddress { get; set; }

        [DisplayName("Ngày bắt đầu")]
        public DateOnly? ProjectStartDate { get; set; }

        [DisplayName("Ngày kết thúc")]
        public DateOnly? ProjectEndDate { get; set; }

        [DisplayName("Trạng thái")]
        public string? ProjectStatus { get; set; }

        [DisplayName("Mô tả")]
        public string? ProjectDescription { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime? CreateDate { get; set; }

        [DisplayName("Người tạo")]
        public string? CreateBy { get; set; }

        [DisplayName("Số ngày còn lại")]
        public int? DaysRemaining { get; set; }

        [DisplayName("Số sản phẩm")]
        public int ProductCount => Products.Count;

        public ReadOnlyCollection<ProductDto> Products { get; set; } = null!;
    }
}
