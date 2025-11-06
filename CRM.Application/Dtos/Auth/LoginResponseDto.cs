using CRM.Application.Dtos.User;

namespace CRM.Application.Dtos.Auth
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public AccountDto Account { get; set; }
    }
}
