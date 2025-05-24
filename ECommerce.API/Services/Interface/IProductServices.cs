using ECommerce.API.Models.DTO.Product;

namespace ECommerce.API.Services.Interface
{
    public interface IProductServices
    {
        Task<DetailProductDTO?> GetProductDetailAsync(int id);
        Task<bool> PauseSalesAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
