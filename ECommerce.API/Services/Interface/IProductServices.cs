using ECommerce.API.Models.DTO.Product;

namespace ECommerce.API.Services.Interface
{
    public interface IProductServices
    {
        Task<DetailProductDTO?> GetProductDetailAsync(int id);
    }
}
