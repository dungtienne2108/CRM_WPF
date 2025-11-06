using CRM.Application.Dtos.Auth;
using CRM.Application.Dtos.User;
using CRM.Shared.Results;

namespace CRM.Application.Interfaces.Auth
{
    public interface IAuthenticationService
    {
        Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request, string ipAddress, CancellationToken cancellationToken = default);
        Task<Result> ChangePasswordAsync(string email, string newPassword, CancellationToken cancellationToken = default);
        Task<Result> LogoutAsync(string accountId, string refreshToken, CancellationToken cancellationToken = default);
        Task<Result<AccountDto>> GetCurrentUserAsync(int accountId, CancellationToken cancellationToken = default);
        Task<bool> ValidateTokenAsync(string token);
    }
}
