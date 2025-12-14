using CRM.Application.Dtos.Auth;
using CRM.Application.Dtos.User;
using CRM.Application.Interfaces.Auth;
using CRM.Domain.Interfaces;
using CRM.Shared.Results;

namespace CRM.Application.Services
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IPasswordService _passwordService;
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationService(
            IPasswordService passwordService,
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork)
        {
            _passwordService = passwordService;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> ChangePasswordAsync(string email, string newPassword, CancellationToken cancellationToken = default)
        {
            try
            {
                var account = await _accountRepository.GetByUserNameAsync(email, cancellationToken);
                if (account == null)
                {
                    return Result.Failure(new Error("USER_NOT_FOUND", $"Không tìm thấy user với email : {email}"));
                }

                if (string.IsNullOrEmpty(newPassword))
                {
                    return Result.Failure(new Error("PASSWORD_IS_INVALID", $"Mật khẩu mới không hợp lệ"));
                }

                var newPasswordHash = await _passwordService.HashPasswordAsync(newPassword);

                if (newPasswordHash == null)
                {
                    return Result.Failure(new Error("PASSWORD_IS_INVALID", $"Mật khẩu mới không hợp lệ"));
                }

                account.PasswordHash = newPasswordHash;
                _accountRepository.Update(account);
                var userChanged = await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (userChanged > 0)
                    return Result.Success();
                else
                    return Result.Failure(new Error("CHANGE_PASSWORD_FAILURE", $"Đổi mật khẩu không thành công"));
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("CHANGE_PASSWORD_FAILURE", $"Lỗi khi đổi pass : {ex.Message}"));
            }
        }

        public Task<Result<AccountDto>> GetCurrentUserAsync(int accountId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request, string ipAddress, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
                {
                    return Result.Failure<LoginResponseDto>(new Error(
                        "INVALID_CREDENTIALS",
                        "Sai thông tin tài khoản hoặc mật khẩu"));
                }

                if (!IsEmail(request.UserName))
                {
                    return Result.Failure<LoginResponseDto>(new Error(
                        "INVALID_CREDENTIALS",
                        "Tài khoản không đúng định dạng!"));
                }

                var account = await _accountRepository.GetByUserNameAsync(request.UserName, cancellationToken);

                if (account is null)
                {
                    return Result.Failure<LoginResponseDto>(new Error(
                        "INVALID_CREDENTIALS",
                        "Sai thông tin tài khoản hoặc mật khẩu"));
                }

                var passwordResult = await _passwordService.VerifyPasswordAsync(request.Password, account.PasswordHash);

                if (!passwordResult)
                {
                    return Result.Failure<LoginResponseDto>(new Error(
                        "INVALID_CREDENTIALS",
                        "Sai thông tin tài khoản hoặc mật khẩu"));
                }

                if (account.Employee == null)
                {
                    return Result.Failure<LoginResponseDto>(new Error(
                        "INVALID_CREDENTIALS",
                        "Tài khoản không hợp lệ!"));
                }

                var accountDto = new AccountDto
                {
                    AccountId = account.AccountId,
                    AccountCode = account.AccountCode,
                    AccountName = account.AccountName,
                    AccountDescription = account.AccountDescription,
                    AccountTypeId = account.AccountTypeId,
                    CreateDate = account.CreateDate,
                    EmployeeId = account.EmployeeId,
                    EmployeeCode = account.Employee.EmployeeCode,
                    EmployeeName = account.Employee.EmployeeName,
                    EmployeeEmail = account.Employee.EmployeeEmail,
                    EmployeePhone = account.Employee.EmployeePhone,
                    EmployeeBirthDay = account.Employee.EmployeeBirthDay,
                    EmployeeLevelId = account.Employee.EmployeeLevelId != null ? account.Employee.EmployeeLevelId.Value : 0
                };

                var loginRespone = new LoginResponseDto
                {
                    AccessToken = Guid.NewGuid().ToString(),
                    RefreshToken = Guid.NewGuid().ToString(),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    Account = accountDto
                };

                return Result.Success(loginRespone);
            }
            catch (Exception ex)
            {
                return Result.Failure<LoginResponseDto>(new Error("LOGIN_FAILED", $"Lỗi xảy ra khi đăng nhập : {ex.Message}"));
            }
        }

        public Task<Result> LogoutAsync(string accountId, string refreshToken, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        private bool IsEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
