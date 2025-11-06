using AutoMapper;
using CRM.Application.Dtos.Employee;
using CRM.Application.Interfaces.Auth;
using CRM.Application.Interfaces.Employee;
using CRM.Domain.Filters.Base;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.Extensions.Caching.Memory;

namespace CRM.Application.Services
{
    public class EmployeeService(IEmployeeRepository employeeRepository,
        IAccountRepository accountRepository,
        IRepository<Gender> genderRepository,
        IRepository<EmployeeLevel> employeeLevelRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IMemoryCache memoryCache,
        IPasswordService passwordService) : IEmployeeService
    {
        public async Task<Result<int>> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            try
            {
                var gender = await genderRepository.GetByIdAsync(request.GenderId);
                if (gender == null)
                {
                    return Result.Failure<int>(new("GENDER_NOT_FOUND", "Không tìm thấy thông tin giới tính"));
                }

                var employeeLevel = await employeeLevelRepository.GetByIdAsync(request.EmployeeLevelId);
                if (employeeLevel == null)
                {
                    return Result.Failure<int>(new("LEVEL_NOT_FOUND", "Không tìm thấy thông tin chức vụ"));
                }

                var existingByEmail = await employeeRepository.GetByEmailAsync(request.EmployeeEmail);
                if (existingByEmail != null)
                {
                    return Result.Failure<int>(new("EMAIL_EXISTS", "Email đã được sử dụng"));
                }

                var existingByIdentityCard = await employeeRepository.GetByIdentityCardAsync(request.EmployeeIdentityCard);
                if (existingByIdentityCard != null)
                {
                    return Result.Failure<int>(new("IDENTITY_CARD_EXISTS", "Chứng minh nhân dân/Căn cước công dân đã được sử dụng"));
                }

                // kiểm tra ngày sinh phải hơn 18 tuổi
                if (DateOnly.FromDateTime(request.EmployeeBirthday) > DateOnly.FromDateTime(DateTime.Now).AddYears(-18))
                {
                    return Result.Failure<int>(new("INVALID_BIRTHDAY", "Nhân viên phải từ 18 tuổi trở lên"));
                }

                var employee = new Employee
                {
                    EmployeeName = request.EmployeeName,
                    GenderId = request.GenderId,
                    EmployeeLevelId = request.EmployeeLevelId,
                    EmployeeBirthDay = DateOnly.FromDateTime(request.EmployeeBirthday),
                    EmployeeAddress = request.EmployeeAddress,
                    EmployeePhone = request.EmployeePhoneNumber,
                    EmployeeEmail = request.EmployeeEmail,
                    EmployeeIdentityCard = request.EmployeeIdentityCard,
                    EmployeeDescription = request.EmployeeDescription
                };

                await employeeRepository.AddAsync(employee);

                // tạo 1 tài khoản
                var existingAccountByEmail = await accountRepository.GetByUserNameAsync(request.EmployeeEmail);
                if (existingAccountByEmail != null)
                {
                    return Result.Failure<int>(new("ACCOUNT_EMAIL_EXISTS", "Tài khoản với email này đã tồn tại"));
                }

                var randomNumber = new Random().Next(100000, 999999).ToString();
                var passwordHash = await passwordService.HashPasswordAsync("Nhanvien@123");

                var account = new Account
                {
                    AccountName = request.EmployeeEmail,
                    PasswordHash = passwordHash,
                    AccountTypeId = 2, // loại tài khoản nhân viên
                    Employee = employee
                };

                await accountRepository.AddAsync(account);

                var added = await unitOfWork.SaveChangesAsync();

                if (added > 0)
                {
                    return Result.Success(employee.EmployeeId);
                }
                else
                {
                    return Result.Failure<int>(new("CREATE_EMPLOYEE_FAILED", "Tạo nhân viên thất bại"));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<int>(new("CREATE_EMPLOYEE_EXCEPTION", $"Lỗi xảy ra khi tạo nhân viên: {ex.Message}"));
            }
        }

        public async Task<Result> DeleteEmployeeAsync(int employeeId)
        {
            try
            {
                var employee = await employeeRepository.GetByIdAsync(employeeId);
                if (employee == null)
                {
                    return Result.Failure(new Error("EMPLOYEE_NOT_FOUND", "Không tìm thấy nhân viên"));
                }
                var account = await accountRepository.GetByEmployeeIdAsync(employeeId);

                if (account == null)
                {
                    return Result.Failure(new Error("ACCOUNT_NOT_FOUND", "Không tìm thấy tài khoản nhân viên"));
                }

                accountRepository.Remove(account);
                employeeRepository.Remove(employee);

                var deleted = await unitOfWork.SaveChangesAsync();
                if (deleted > 0)
                {
                    // xóa cache
                    //memoryCache.Remove($"Employee_{employeeId}");
                    return Result.Success();
                }
                else
                {
                    return Result.Failure(new Error("DELETE_EMPLOYEE_FAILED", "Xóa nhân viên thất bại"));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("DELETE_EMPLOYEE_EXCEPTION", $"Lỗi xảy ra khi xóa nhân viên: {ex.Message}"));
            }
        }

        public async Task<Result<IEnumerable<EmployeeLevelOption>>> GetAllEmployeeLevelsAsync()
        {
            try
            {
                //if (memoryCache.TryGetValue("EmployeeLevels", out IEnumerable<EmployeeLevelOption>? cachedLevels))
                //{
                //    return Result.Success(cachedLevels);
                //}

                var employeeLevels = await employeeLevelRepository.GetAllAsync();

                var employeeLevelOptions = employeeLevels.Select(el => new EmployeeLevelOption
                {
                    Id = el.EmployeeLevelId,
                    Name = el.EmployeeLevelName
                });

                //memoryCache.Set("EmployeeLevels", employeeLevelOptions, TimeSpan.FromHours(1));

                return Result.Success(employeeLevelOptions);
            }
            catch (Exception)
            {
                return Result.Failure<IEnumerable<EmployeeLevelOption>>(new("GET_LEVEL_FAILED", "Lỗi khi lấy danh sách chức vụ"));
            }
        }

        public async Task<PagedResult<EmployeeDto>> GetAllEmployeesAsync(GetEmployeeRequest request)
        {
            try
            {
                //if (memoryCache.TryGetValue($"EmployeeList_{request.Keyword}_{request.PageNumber}_{request.PageSize}", out PagedResult<EmployeeDto>? cachedEmployees))
                //{
                //    return cachedEmployees;
                //}

                var filter = new EmployeeFilter
                {
                    Keyword = request.Keyword,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                var employees = await employeeRepository.GetAllEmployeesAsync(filter);

                var employeeDtos = mapper.Map<List<EmployeeDto>>(employees.Items);

                var pagedResult = new PagedResult<EmployeeDto>(employeeDtos, employees.TotalCount, employees.PageNumber, employees.PageSize);

                return pagedResult;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Lỗi khi lấy nhân viên:", ex);
            }
        }

        public async Task<Result<IEnumerable<GenderOption>>> GetAllGendersAsync()
        {
            try
            {
                //if (memoryCache.TryGetValue("Genders", out IEnumerable<GenderOption>? cachedGenders))
                //{
                //    return Result.Success(cachedGenders);
                //}

                var genders = await genderRepository.GetAllAsync();

                var genderOptions = genders.Select(g => new GenderOption
                {
                    Id = g.GenderId,
                    Name = g.GenderName
                });

                //memoryCache.Set("Genders", genderOptions, TimeSpan.FromHours(1));

                return Result.Success(genderOptions);
            }
            catch (Exception)
            {
                return Result.Failure<IEnumerable<GenderOption>>(new("GET_GENDER_FAILED", "Lỗi khi lấy danh sách giới tính"));
            }
        }

        public async Task<Result<EmployeeDto>> GetEmployeeByIdAsync(int employeeId)
        {
            try
            {
                //if (memoryCache.TryGetValue($"Employee_{employeeId}", out EmployeeDto? cachedEmployee))
                //{
                //    return Result.Success(cachedEmployee);
                //}

                var employee = await employeeRepository.GetEmployeeByIdAsync(employeeId);

                if (employee == null)
                {
                    return Result.Failure<EmployeeDto>(new Error("EMPLOYEE_NOT_FOUND", $"Không tìm thấy nhân viên với Id: {employeeId}"));
                }

                var employeeDto = mapper.Map<EmployeeDto>(employee);

                //memoryCache.Set($"Employee_{employeeId}", employeeDto, TimeSpan.FromMinutes(10));

                return Result.Success(employeeDto);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Lỗi khi lấy nhân viên với Id: {employeeId}", ex);
            }
        }

        public async Task<Result<EmployeeDto>> UpdateEmployeeAsync(UpdateEmployeeRequest request)
        {
            try
            {
                var employee = await employeeRepository.GetEmployeeByIdAsync(request.Id);
                if (employee == null)
                {
                    return Result.Failure<EmployeeDto>(new("EMPLOYEE_NOT_FOUND", "Không tìm thấy nhân viên"));
                }

                employee.EmployeeName = request.Name;
                employee.GenderId = request.GenderId;
                employee.EmployeeLevelId = request.LevelId;
                employee.EmployeeBirthDay = DateOnly.FromDateTime(request.DateOfBirth);
                employee.EmployeeAddress = request.Address;
                employee.EmployeePhone = request.Phone;
                employee.EmployeeEmail = request.Email;
                employee.EmployeeDescription = request.Description;
                employee.EmployeeIdentityCard = request.IdentityCard;

                employeeRepository.Update(employee);

                var updated = await unitOfWork.SaveChangesAsync();
                if (updated > 0)
                {
                    // xóa cache
                    var employeeDto = mapper.Map<EmployeeDto>(employee);
                    return Result.Success(employeeDto);
                }
                else
                {
                    return Result.Failure<EmployeeDto>(new("UPDATE_EMPLOYEE_FAILED", "Cập nhật nhân viên thất bại"));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<EmployeeDto>(new("UPDATE_EMPLOYEE_EXCEPTION", $"Lỗi xảy ra khi cập nhật nhân viên: {ex.Message}"));
            }
        }
    }
}
