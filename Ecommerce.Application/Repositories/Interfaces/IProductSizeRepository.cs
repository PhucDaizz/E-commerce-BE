using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IProductSizeRepository
    {
        Task<ProductSizes> CreateAsync(ProductSizes productSizes);
        Task<ProductSizes?> UpdateAsync(ProductSizes productSizes);
        Task<ProductSizes?> DeleteAsync(int ProductSizeID);
        Task<ProductSizes?> GetByIdAsync(int ProductSizeID);
        Task<IEnumerable<ProductSizes>?> GetAllByColorAsync(int id);
        Task<bool> IsExistAsync(int ProductSizeID, string Size);
        Task<ProductSizes?> AddAsync(ProductSizes productSizes);
        Task<bool> DeleteByColorIDAsync(int colorID);
        Task<ProductSizes?> DeleteByColorAndSizeAsync(int colorID, string size);
        Task<bool> UpdateRangeAsync(IEnumerable<CartItems> cartItems);
        Task<bool> UpsertRangeAsync(IEnumerable<ProductSizes> productSizesToUpsert);
        Task<bool> ReturnStockOnCancel(Dictionary<int, int> productSize);
        Task<IEnumerable<ProductSizes>> GetByIdsAsync(List<int> productSizeIds);
    }
}
