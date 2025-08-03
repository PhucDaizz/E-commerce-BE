using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ECommerce.API.Services.Impemention
{
    public class ProductImageServices : IProductImageServices
    {
        private readonly AppDbContext dbContext;
        private readonly IWebHostEnvironment environment;
        private readonly Cloudinary _cloudinary;
        private readonly IProductImageRepository productImageRepository;

        public ProductImageServices(AppDbContext dbContext, IWebHostEnvironment environment, Cloudinary cloudinary, IProductImageRepository productImageRepository)
        {
            this.dbContext = dbContext;
            this.environment = environment;
            _cloudinary = cloudinary;
            this.productImageRepository = productImageRepository;
        }
        public async Task<bool> DeleteImage(string fileNameWithExtension)
        {
            if (string.IsNullOrEmpty(fileNameWithExtension))
            {
                throw new ArgumentNullException(nameof(fileNameWithExtension));
            }

            if (fileNameWithExtension.Contains("cloudinary.com"))
            {
                var publicId = GetPublicIdFromUrl(fileNameWithExtension);

                var deleteParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Image
                };

                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.Result != "ok")
                {
                    return false;
                }

                return true;
            }

            var contentPath = environment.ContentRootPath;
            var path = Path.Combine(contentPath, $"Uploads", fileNameWithExtension);
            if (!File.Exists(path))
            {
                throw new ArgumentNullException($"Invalid file path");
            }
            File.Delete(path);
            return true;
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

        public async Task<IEnumerable<string>> SaveImagesAsync(IEnumerable<IFormFile> imageFiles, string[] allowedFileExtensions, Products product, int productId, int length, bool? onCloud = false)
        {

            if (!imageFiles.Any() || imageFiles == null)
            {
                return new List<string>();
            }

            List<string> imageUrls = new List<string>();


            // Nếu lưu trên Cloudinary
            if (onCloud == true)
            {
                string[]? category = new string[] { product.Categories?.CategoryName ?? "default" };

                foreach (var file in imageFiles)
                {
                    if (length >= 5) break;

                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedFileExtensions.Contains(ext))
                    {
                        throw new NotSupportedException($"Only {string.Join(", ", allowedFileExtensions)} are allowed");
                    }

                    await using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        PublicId = $"product/{Slugify(product.ProductName)}_{Guid.NewGuid()}",
                        Overwrite = true,
                        UseFilename = true,
                        UniqueFilename = false,
                        Tags = string.Join(",", category)
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        imageUrls.Add(uploadResult.SecureUri.ToString());
                        length++;
                    }
                    else
                    {
                        throw new Exception("Image upload failed");
                    }
                }

                return imageUrls;
            }

            // Nếu lưu local
            var contentPath = environment.ContentRootPath;
            var path = Path.Combine(contentPath, "Uploads");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            foreach (var imageFile in imageFiles)
            {

                if (imageFile.Length < 0 || length > 5) continue;

                // Check the allow extenstions
                var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowedFileExtensions.Contains(ext))
                {
                    throw new NotImplementedException($"Only {string.Join(",", allowedFileExtensions)} are allowed");
                }

                var slugifiedProductName = Slugify(product.ProductName);
                var fileName = $"{slugifiedProductName}_{productId}_{Guid.NewGuid().ToString()}{ext}";
                var fileNameWithPath = Path.Combine(path, fileName);
                using var stream = new FileStream(fileNameWithPath, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                imageUrls.Add(fileName);
                length++;
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

        // / Upload image to Cloudinary
        public async Task<string> UploadImageCloundinaryAsync(IFormFile file, string[] allowedFileExtensions, string name, string category, string? path = "product")
        {
            if(file == null || file.Length == 0)
            {
                throw new ArgumentNullException(nameof(file), "File cannot be null or empty");
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedFileExtensions.Contains(ext))
            {
                throw new NotSupportedException($"Only {string.Join(", ", allowedFileExtensions)} are allowed");
            }

            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = $"{path}/{Slugify(name)}_{Guid.NewGuid()}",
                Overwrite = true,
                UseFilename = true,
                UniqueFilename = false,
                Tags = !string.IsNullOrEmpty(category) ? string.Join(",", new[] { category }) : null
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUri.ToString();
            }
            throw new Exception("Image upload failed");
        }
        
        // Upload multiple images to Cloudinary
        public async Task<IEnumerable<object>> UploadListImagesCloudinaryAsync(IEnumerable<IFormFile> files, string[] allowedFileExtensions, string name, string[]? category, string? path = "product")
        {
            if(files == null || !files.Any())
            {
                throw new ArgumentNullException(nameof(files), "File cannot be null or empty");
            }

            foreach (var file in files)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedFileExtensions.Contains(ext))
                {
                    throw new NotSupportedException($"Only {string.Join(", ", allowedFileExtensions)} are allowed");
                }
            }

            var uploadResults = new List<object>();

            foreach (var file in files)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = $"{path}/{Slugify(name)}_{Guid.NewGuid()}",
                    Overwrite = true,
                    UseFilename = true,
                    UniqueFilename = false,
                    Tags = category != null ? string.Join(",", category) : null
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    uploadResults.Add(uploadResult.SecureUri.ToString());
                }
                else
                {
                    throw new Exception("Image upload failed");
                }
            }
            return uploadResults;
        }

        public async Task<bool> retainProductFeaturedImage(int productId)
        {
            var images = await dbContext.ProductImages
                .Where(x => x.ProductID == productId)
                .OrderBy(x => x.IsPrimary)
                .ThenBy(x => x.CreatedAt)
                .ToListAsync();

            if (images.Count <= 1)
                return false;

            var imagesToDelete = images.Skip(1).ToList();

            foreach (var image in imagesToDelete)
            {
                if (!string.IsNullOrEmpty(image.ImageURL))
                {
                    if (image.ImageURL.Contains("cloudinary.com"))
                    {
                        var publicId = GetPublicIdFromUrl(image.ImageURL);

                        var deleteParams = new DeletionParams(publicId)
                        {
                            ResourceType = ResourceType.Image
                        };

                        var result = await _cloudinary.DestroyAsync(deleteParams);
                        if (result.Result != "ok" && result.Result != "not found")
                        {
                            throw new Exception($"Xoá ảnh Cloudinary thất bại: {result.Result}");
                        }
                    }
                    else
                    {
                        var path = Path.Combine(environment.ContentRootPath, "Uploads", image.ImageURL);
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                }
            }

            dbContext.ProductImages.RemoveRange(imagesToDelete);
            //await dbContext.SaveChangesAsync(); UOW

            return true;
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

        public async Task<bool> DeleteProductImagesAsync(int productId)
        {
            var images = await dbContext.ProductImages.Where(x => x.ProductID == productId).ToListAsync();
            foreach (var image in images)
            {
                if (!string.IsNullOrEmpty(image.ImageURL))
                {
                    // Detele cloudinary image
                    if (image.ImageURL.Contains("cloudinary.com"))
                    {
                        var publicId = GetPublicIdFromUrl(image.ImageURL);

                        var deleteParams = new DeletionParams(publicId)
                        {
                            ResourceType = ResourceType.Image
                        };

                        var result = await _cloudinary.DestroyAsync(deleteParams);
                        if (result.Result != "ok")
                        {
                            throw new Exception($"Xoá ảnh Cloudinary thất bại: {result.Result}");
                        }
                    }
                    // Delete local image
                    else 
                    {
                        var contentPath = environment.ContentRootPath;
                        var path = Path.Combine(contentPath, "Uploads", image.ImageURL);
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                    
                }

            }
            dbContext.ProductImages.RemoveRange(images);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
