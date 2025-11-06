using CRM.Application.Dtos.User;

namespace CRM.Application.Interfaces
{
    public interface ISessionService
    {
        AccountDto CurrentAccount { get; }
        string AccessToken { get; }
        string RefreshToken { get; }
        bool IsAuthenticated { get; }

        void SetCurrentAccount(AccountDto account);
        void SetRefreshToken(string refreshToken);
        void SetAccessToken(string accessToken);

    }
}
