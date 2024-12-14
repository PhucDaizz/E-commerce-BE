using ECommerce.API.Models.Domain;

namespace ECommerce.API.Services.Interface
{
    public interface IProductImageServices
    {
        Task<string> SaveImageAsync(IFormFile imageFile, string[] allowedFileExtensions, string productName, int productId);
        void DeleteImage(string fileNameWithExtension); 
        Task<IEnumerable<string>> SaveImagesAsync(IEnumerable<IFormFile> imagefiles, string[] allowedFileExtensions,string productName,int productId);
    }
}
