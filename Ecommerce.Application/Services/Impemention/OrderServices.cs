using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Services.Impemention
{
    public class OrderServices : IOrderServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> CanncelOrderAsync(string orderId, string userId, bool isAdmin = false)
        {

            if (string.IsNullOrEmpty(orderId))
            {
                throw new ArgumentException("Order ID cannot be null or empty.");
            }

            if (!Guid.TryParse(orderId, out var orderGuid))
                throw new ArgumentException("Invalid Order ID format.");

            var order = await _unitOfWork.Orders.GetByIdAdminAsync(orderGuid);
            if (order == null)
                throw new ArgumentException("Order not found.");

            if (order.Status == (int)OrderStatus.Confirmed)
                throw new InvalidOperationException("Order already confirmed and cannot be cancelled.");

            if (order.Status == (int)OrderStatus.Cancelled)
                throw new InvalidOperationException("Order already cancelled.");

            if (!isAdmin && order.UserID.ToString() != userId)
                throw new UnauthorizedAccessException("Not order owner.");

            await _unitOfWork.Orders.UpdateOrderStatus(orderGuid, (int)OrderStatus.Cancelled);

            var orderDetails = await _unitOfWork.OrderDetails.GetListOrderDetailsAsync(orderGuid);

            var productSizeQuantities = orderDetails
                .GroupBy(d => d.ProductSizeId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            await _unitOfWork.ProductSizes.ReturnStockOnCancel(productSizeQuantities);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
