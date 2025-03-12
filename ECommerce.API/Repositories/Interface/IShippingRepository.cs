using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Shipping;

namespace ECommerce.API.Repositories.Interface
{
    public interface IShippingRepository
    {
        Task<Shippings> CreateAsync(Shippings shipping);
        Task<Shippings?> UpdateAsync(Guid orderId, UpdateShippingDTO shipping);
    }
}
