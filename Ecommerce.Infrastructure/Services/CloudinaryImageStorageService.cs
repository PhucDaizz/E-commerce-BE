using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Ecommerce.Infrastructure.Services
{
    public class CloudinaryImageStorageService : IFileStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageStorageService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }
        public async Task DeleteFileAsync(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath) || !relativePath.Contains("cloudinary.com")) return;
            var publicId = GetPublicIdFromUrl(relativePath);
            var deleteParams = new DeletionParams(publicId) { ResourceType = ResourceType.Image };
            await _cloudinary.DestroyAsync(deleteParams);
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string originalFileName, string subFolder, string fileNamePrefix)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(originalFileName, fileStream),
                PublicId = $"{subFolder}/{fileNamePrefix}_{Guid.NewGuid()}"
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("Cloudinary upload failed.");
            return uploadResult.SecureUri.ToString();
        }

        private string GetPublicIdFromUrl(string url)
        {
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/');

            var uploadIndex = Array.IndexOf(segments, "upload");

            if (uploadIndex >= 0 && uploadIndex + 2 < segments.Length)
            {
                var relevantParts = segments.Skip(uploadIndex + 2); // Bỏ cả "upload" và version
                var publicIdWithExtension = string.Join("/", relevantParts);
                var publicId = Path.ChangeExtension(publicIdWithExtension, null); // Bỏ .jpg
                return publicId;
            }

            throw new InvalidOperationException("Không thể tách public_id từ URL");
        }
    }
}
