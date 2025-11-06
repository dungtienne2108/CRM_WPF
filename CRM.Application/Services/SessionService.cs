using CRM.Application.Dtos.User;
using CRM.Application.Interfaces;

namespace CRM.Application.Services
{
    public sealed class SessionService : ISessionService
    {
        private AccountDto _currentAccount;
        private string _accessToken;
        private string _refreshToken;

        public AccountDto CurrentAccount => _currentAccount;

        public string AccessToken => _accessToken;

        public string RefreshToken => _refreshToken;

        public bool IsAuthenticated => _currentAccount != null && !string.IsNullOrEmpty(_accessToken);

        public void SetAccessToken(string accessToken)
        {
            _accessToken = accessToken;
        }

        public void SetCurrentAccount(AccountDto account)
        {
            _currentAccount = account;
        }

        public void SetRefreshToken(string refreshToken)
        {
            _refreshToken = refreshToken;
        }
    }
}
