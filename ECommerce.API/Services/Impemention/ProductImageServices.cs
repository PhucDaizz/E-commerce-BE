using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Services.Interface;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ECommerce.API.Services.Impemention
{
    public class ProductImageServices : IProductImageServices
    {
        private readonly ECommerceDbContext dbContext;
        private readonly IWebHostEnvironment environment;

        public ProductImageServices(ECommerceDbContext dbContext, IWebHostEnvironment environment)
        {
            this.dbContext = dbContext;
            this.environment = environment;
        }
        public void DeleteImage(string fileNameWithExtension)
        {
            if (string.IsNullOrEmpty(fileNameWithExtension))
            {
                throw new ArgumentNullException(nameof(fileNameWithExtension));
            }
            var contentPath = environment.ContentRootPath;
            var path = Path.Combine(contentPath, $"Uploads", fileNameWithExtension);
            if (!File.Exists(path))
            {
                throw new ArgumentNullException($"Invalid file path");
            }
            File.Delete(path);
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile, string[] allowedFileExtensions, string productName, int productId)
        {
            if (imageFile == null || imageFile.Length < 0)
            {
                return null;
            }

            var contentPath = environment.ContentRootPath;
            var path = Path.Combine(contentPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Check the allow extenstions
            var ext = Path.GetExtension(imageFile.FileName);
            if (!allowedFileExtensions.Contains(ext))
            {
                throw new NotImplementedException($"Only {string.Join(",", allowedFileExtensions)} are allowed");
            }

            // generate a unique filename
            var slugifiedProductName = Slugify(productName);
            var fileName = $"{slugifiedProductName}_{productId}_{Guid.NewGuid().ToString()}{ext}";
            var fileNameWithPath = Path.Combine(path, fileName);
            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await imageFile.CopyToAsync(stream);
            return fileName;
        }

        public async Task<IEnumerable<string>> SaveImagesAsync(IEnumerable<IFormFile> imageFiles, string[] allowedFileExtensions, string productName, int productId)
        {

            if (!imageFiles.Any() || imageFiles == null)
            {
                return new List<string>();
            }

            List<string> imageUrls = new List<string>();

            var contentPath = environment.ContentRootPath;
            var path = Path.Combine(contentPath, "Uploads");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (var imageFile in imageFiles)
            {

                if (imageFile.Length < 0) continue;

                // Check the allow extenstions
                var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowedFileExtensions.Contains(ext))
                {
                    throw new NotImplementedException($"Only {string.Join(",", allowedFileExtensions)} are allowed");
                }

                var slugifiedProductName = Slugify(productName);
                var fileName = $"{slugifiedProductName}_{productId}_{Guid.NewGuid().ToString()}{ext}";
                var fileNameWithPath = Path.Combine(path, fileName);
                using var stream = new FileStream(fileNameWithPath, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                imageUrls.Add(fileName);
            }

            return imageUrls;
            //continue
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
