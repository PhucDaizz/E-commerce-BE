using ECommerce.API.Models.Domain;

namespace ECommerce.API.Services.Interface
{
    public interface IProductImageServices
    {
        Task<string> SaveImageAsync(IFormFile imageFile, string[] allowedFileExtensions, string productName, int productId);
        void DeleteImage(string fileNameWithExtension); 
        Task<IEnumerable<string>> SaveImagesAsync(IEnumerable<IFormFile> imagefiles, string[] allowedFileExtensions,Products product,int productId, int length, bool? onCloud = false);
        Task<string> UploadImageCloundinaryAsync(IFormFile file, string[] allowedFileExtensions, string name, string? category, string? path = "product");
        Task<IEnumerable<object>> UploadListImagesCloudinaryAsync(IEnumerable<IFormFile> files, string[] allowedFileExtensions, string name, string[]? category, string? path = "product");


    }
}
