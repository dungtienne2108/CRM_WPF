using System.ComponentModel;

namespace CRM.Application.Dtos.Project
{
    public sealed class ProductDto
    {
        [DisplayName("ID")]
        public int ProductId { get; set; }

        [DisplayName("Mã code")]
        public string? ProductCode { get; set; }

        [DisplayName("Tên sản phẩm")]
        public string ProductName { get; set; } = null!;

        [DisplayName("Số tầng")]
        public int ProductFloors { get; set; }

        [DisplayName("Địa chỉ sản phẩm")]
        public string? ProductAddress { get; set; }

        [DisplayName("Số thứ tự sản phẩm")]
        public int? ProductNumber { get; set; }

        [DisplayName("Diện tích (m²)")]
        public decimal? ProductArea { get; set; }

        [DisplayName("Giá sản phẩm (VNĐ)")]
        public decimal? ProductPrice { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime? CreateDate { get; set; }

        [DisplayName("Người tạo")]
        public string? CreateBy { get; set; }

        public int ProductTypeId { get; set; }

        [DisplayName("Loại sản phẩm")]
        public string ProductTypeName { get; set; } = null!;

        public int? ProjectId { get; set; }

        [DisplayName("Tên dự án")]
        public string ProjectName { get; set; } = null!;

        public int ProductStatusId { get; set; }

        [DisplayName("Trạng thái")]
        public string ProductStatusName { get; set; } = null!;
    }
}
