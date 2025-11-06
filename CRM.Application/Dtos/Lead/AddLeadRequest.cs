namespace CRM.Application.Dtos.Lead
{
    public sealed class AddLeadRequest
    {
        //Tên
        public string Name { get; set; } = null!;

        //Công ty
        public string? Company { get; set; }

        //Điện thoại
        public string? Phone { get; set; }

        //Email
        public string? Email { get; set; }

        // Địa chỉ
        public string? Address { get; set; }

        // Nguồn
        public int? SourceId { get; set; }

        // Mức độ tiềm năng
        public int PotentialLevelId { get; set; }

        // Trạng thái
        public int StatusId { get; set; }

        // nhân sự phụ trách
        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
