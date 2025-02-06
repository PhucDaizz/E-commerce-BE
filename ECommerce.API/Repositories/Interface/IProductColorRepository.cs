using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.ProductColor;

namespace ECommerce.API.Repositories.Interface
{
    public interface IProductColorRepository
    {
        Task<ProductColors?> GetByIdAsync(int id);
        Task<ProductColors> CreateAsync(ProductColors productColors);
        Task<IEnumerable<ProductColors>> CreateRangeAsync(IEnumerable<ProductColors> productColors);
        Task<ProductColors?> UpdateAsync(ProductColors productColors);
        Task<ProductColors?> DeleteAsync(int id);
        Task<IEnumerable<ProductColors>> GetAllAsync();
        Task<IEnumerable<ProductColors>> GetProductColorSizeAsync(int productId);

    }
}
