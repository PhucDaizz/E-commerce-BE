using Ecommerce.Application.Services.Contracts.Infrastructure;

namespace Ecommerce.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _webRootPath;

        public LocalFileStorageService(string webRootPath)
        {
            if (string.IsNullOrEmpty(webRootPath))
            {
                throw new ArgumentNullException(nameof(webRootPath));
            }
            _webRootPath = webRootPath;
        }
        public Task DeleteFileAsync(string fileUrlOrPath)
        { 
            if (string.IsNullOrEmpty(fileUrlOrPath)) return Task.CompletedTask;

            var filePath = Path.Combine(_webRootPath, fileUrlOrPath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return Task.CompletedTask;
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string originalFileName, string subFolder, string fileNamePrefix)
        {
            var ext = Path.GetExtension(originalFileName);
            // Tạo tên file có ý nghĩa
            var fileName = $"{fileNamePrefix}_{Guid.NewGuid()}{ext}";

            var targetFolderPath = Path.Combine(_webRootPath, subFolder);
            Directory.CreateDirectory(targetFolderPath);
            var filePath = Path.Combine(targetFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            return $"/{subFolder.Replace('\\', '/')}/{fileName}";
        }
    }
}
