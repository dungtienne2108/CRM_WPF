namespace CRM.Application.Dtos.Employee
{
    public class UpdateEmployeeRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IdentityCard { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int GenderId { get; set; }
        public int LevelId { get; set; }
        public string? Description { get; set; } = string.Empty;
    }
}
