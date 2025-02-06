using ECommerce.API.Models.DTO.ProductSize;

namespace ECommerce.API.Services.Interface
{
    public interface IProductSizeServices
    {
        Task<ProductSizeResponse> CreateRangeAsync(CreateProductSizesDTO productSizesDTO);
    }
}
