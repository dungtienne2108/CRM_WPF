using CRM.Application.Dtos.Employee;
using CRM.Shared.Results;

namespace CRM.Application.Interfaces.Employee
{
    public interface IEmployeeService
    {
        Task<Result> DeleteEmployeeAsync(int employeeId);
        Task<Result<EmployeeDto>> UpdateEmployeeAsync(UpdateEmployeeRequest request);
        Task<Result<int>> CreateEmployeeAsync(CreateEmployeeRequest request);
        Task<Result<IEnumerable<GenderOption>>> GetAllGendersAsync();
        Task<Result<IEnumerable<EmployeeLevelOption>>> GetAllEmployeeLevelsAsync();
        Task<Result<EmployeeDto>> GetEmployeeByIdAsync(int employeeId);
        Task<PagedResult<EmployeeDto>> GetAllEmployeesAsync(GetEmployeeRequest request);
    }
}
