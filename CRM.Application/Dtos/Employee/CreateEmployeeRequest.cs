namespace CRM.Application.Dtos.Employee
{
    public class CreateEmployeeRequest
    {
        public string EmployeeName { get; set; }
        public int EmployeeLevelId { get; set; }
        public int GenderId { get; set; }
        public string EmployeeIdentityCard { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeePhoneNumber { get; set; }
        public string EmployeeAddress { get; set; }
        public DateTime EmployeeBirthday { get; set; }
        public string? EmployeeDescription { get; set; }

    }
}
