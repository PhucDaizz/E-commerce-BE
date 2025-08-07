using Ecommerce.Application.DTOS.Shipping;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Services.Impemention
{
    public class ShippingServices : IShippingServices
    {
        private readonly IShippingRepository _shippingRepository;
        private readonly IOrderRepository _orderRepository;

        public ShippingServices(IShippingRepository shippingRepository, IOrderRepository orderRepository)
        {
            _shippingRepository = shippingRepository;
            _orderRepository = orderRepository;
        }
        public async Task<Shippings> UpdateShippingAfterCreateAsync(Guid orderId, UpdateShippingDTO shipping)
        {
            var shippings = await _shippingRepository.UpdateAsync(orderId, shipping);
            if (shippings == null)
            {
                return null;
            }
            await _orderRepository.UpdateOrderStatus(orderId, 4);
            return shippings;
        }
    }
}
