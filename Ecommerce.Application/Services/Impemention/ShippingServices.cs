using Ecommerce.Application.DTOS.Shipping;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Services.Impemention
{
    public class ShippingServices : IShippingServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShippingServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Shippings> UpdateShippingAfterCreateAsync(Guid orderId, UpdateShippingDTO shipping)
        {
            var shippings = await _unitOfWork.shipping.UpdateAsync(orderId, shipping);
            if (shippings == null)
            {
                return null;
            }
            await _unitOfWork.Orders.UpdateOrderStatus(orderId, 4);
            await _unitOfWork.SaveChangesAsync();
            return shippings;
        }
    }
}
