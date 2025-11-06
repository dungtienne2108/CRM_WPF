namespace CRM.Application.Interfaces
{
    public interface IUploadService
    {
        Task<string> UploadFileAsync(byte[] fileData, string fileName);
    }
}
