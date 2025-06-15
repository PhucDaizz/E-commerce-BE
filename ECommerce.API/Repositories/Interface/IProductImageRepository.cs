using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.ProductImage;

namespace ECommerce.API.Repositories.Interface
{
    public interface IProductImageRepository
    {
        Task<ProductImages> CreateAsync(ProductImages productImages);
        Task<ProductImages> GetByIdAsync(int id);
        Task<ProductImages?> UpdateAsync(ProductImages productImages);
        Task<ProductImages?> DeleteAsync(int id);
        Task<IEnumerable<ProductImages>> GetAllByProductIDAsync(int productId);
        Task<IEnumerable<ProductImages>> CreateImagesAsync(IEnumerable<ProductImages> imagesList);
        /*Task<bool> retainProductFeaturedImage(IEnumerable<ProductImages> productImages);*/
        Task<bool> DeleteProductImagesAsync(int productId);
    }
}
