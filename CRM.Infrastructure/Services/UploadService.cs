using CRM.Application.Interfaces;

namespace CRM.Infrastructure.Services
{
    public class UploadService : IUploadService
    {
        public UploadService() { }

        public async Task<string> UploadFileAsync(byte[] fileData, string fileName)
        {
            var imageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents");

            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }

            var uniqueName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var filePath = Path.Combine(imageFolder, uniqueName);

            await File.WriteAllBytesAsync(filePath, fileData);

            return filePath;
        }
    }
}
