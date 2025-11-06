namespace CRM.Application.Interfaces.Auth
{
    public interface IPasswordService
    {
        Task<bool> VerifyPasswordAsync(string password, string hashedPassword);
        Task<string> HashPasswordAsync(string password);
    }
}
