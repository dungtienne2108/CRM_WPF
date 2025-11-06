using CRM.Application.Interfaces.Auth;

namespace CRM.Application.Services
{
    public sealed class PasswordService : IPasswordService
    {
        public async Task<string> HashPasswordAsync(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Mật khẩu không được để trống", nameof(password));
            }

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<bool> VerifyPasswordAsync(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
