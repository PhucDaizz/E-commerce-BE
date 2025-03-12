using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Shipping;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;

namespace ECommerce.API.Services.Impemention
{
    public class ShippingServices : IShippingServices
    {
        private readonly IShippingRepository shippingRepository;
        private readonly IOrderRepository orderRepository;

        public ShippingServices( IShippingRepository shippingRepository, IOrderRepository orderRepository)
        {
            this.shippingRepository = shippingRepository;
            this.orderRepository = orderRepository;
        }
        public async Task<Shippings> UpdateShippingAfterCreateAsync(Guid orderId, UpdateShippingDTO shipping)
        {
             var shippings = await shippingRepository.UpdateAsync(orderId, shipping);
            if (shippings == null)
            {
               return null;
            }
            await orderRepository.UpdateOrderStatus(orderId, 4);
            return shippings;
        }
    }
}
