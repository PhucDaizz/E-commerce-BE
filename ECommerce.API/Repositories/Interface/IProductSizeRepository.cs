using ECommerce.API.Models.Domain;

namespace ECommerce.API.Repositories.Interface
{
    public interface IProductSizeRepository
    {
        Task<ProductSizes> CreateAsync(ProductSizes productSizes);
        Task<ProductSizes?> UpdateAsync(ProductSizes productSizes);
        Task<ProductSizes?> DeleteAsync(int ProductSizeID);
        Task<ProductSizes?> GetByIdAsync(int ProductSizeID);
        Task<IEnumerable<ProductSizes>?> GetAllByColorAsync(int id);
    }
}
