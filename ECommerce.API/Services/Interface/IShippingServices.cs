using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Shipping;

namespace ECommerce.API.Services.Interface
{
    public interface IShippingServices
    {
        Task<Shippings> UpdateShippingAfterCreateAsync(Guid orderId, UpdateShippingDTO shipping);
    }
}
