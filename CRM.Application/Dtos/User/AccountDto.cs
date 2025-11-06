namespace CRM.Application.Dtos.User
{
    public sealed class AccountDto
    {
        public int AccountId { get; set; }
        public string? AccountCode { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public int? AccountTypeId { get; set; }
        public string? AccountDescription { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? CreateDate { get; set; }

        // Employee Info
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeEmail { get; set; }
        public string? EmployeePhone { get; set; }
        public DateOnly? EmployeeBirthDay { get; set; }
    }
}
