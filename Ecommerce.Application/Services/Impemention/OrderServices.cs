using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Contracts.Infrastructure;
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

            // Đơn nháp banking chưa trả tiền: chỉ cancel + giải phóng hold theo mã giao dịch.
            // Kho chưa trừ nên KHÔNG hoàn kho (tránh double stock).
            if (order.Status == (int)OrderStatus.Pending && order.PaymentMethodID == (int)PaymentMethod.VNPAY)
            {
                await _unitOfWork.Orders.UpdateOrderStatus(orderGuid, (int)OrderStatus.Cancelled);
                if (!string.IsNullOrEmpty(order.TransactionRef))
                {
                    var holds = await _unitOfWork.InventoryReservations.GetByTransactionIdAsync(order.TransactionRef);
                    if (holds.Any())
                        await _unitOfWork.InventoryReservations.DeleteRangeAsync(holds);
                }
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            if(order.PaymentMethodID == (int)PaymentMethod.VNPAY)
                throw new InvalidOperationException("The order is non-refundable as payment has been processed.");

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
