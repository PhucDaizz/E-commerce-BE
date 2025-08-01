using Ecommerce.Infrastructure.Contracts.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Ecommerce.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public LocalFileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public Task DeleteFileAsync(string fileUrlOrPath)
        {
            if (string.IsNullOrEmpty(fileUrlOrPath)) return Task.CompletedTask;

            var filePath = Path.Combine(_env.WebRootPath, fileUrlOrPath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return Task.CompletedTask;
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string originalFileName, string productName)
        {
            var ext = Path.GetExtension(originalFileName);
            // Tái tạo lại logic tạo tên file cũ nếu bạn muốn giữ sự nhất quán
            var slugifiedProductName = Slugify(productName);
            var fileName = $"{slugifiedProductName}_{Guid.NewGuid()}{ext}";

            var uploadsFolderPath = Path.Combine(_env.WebRootPath, "Uploads");
            Directory.CreateDirectory(uploadsFolderPath);
            var filePath = Path.Combine(uploadsFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            // Trả về đường dẫn web, có dấu / ở đầu để là URL tuyệt đối từ gốc
            return $"/{Path.Combine("Uploads", fileName).Replace('\\', '/')}";
        }

        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string Slugify(string text)
        {
            // Remove diacritics
            var slug = RemoveDiacritics(text);
            // Replace spaces with underscores
            slug = Regex.Replace(slug, @"\s", "_");
            // Remove invalid characters
            slug = Regex.Replace(slug, @"[^a-zA-Z0-9_\-]", "");
            return slug;
        }
    }
}
