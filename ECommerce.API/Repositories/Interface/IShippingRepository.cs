using ECommerce.API.Models.Domain;

namespace ECommerce.API.Repositories.Interface
{
    public interface IShippingRepository
    {
        Task<Shippings> CreateAsync(Shippings shipping);
    }
}
