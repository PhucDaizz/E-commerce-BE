using Ecommerce.Application.Common;
using Ecommerce.Application.DTOS.CartItem;
using Ecommerce.Application.DTOS.Inventory;
using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Services.Impemention
{
    public class InventoryReservationService : IInventoryReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InventoryReservationService> _logger;
        private const int RESERVATION_MINUTES = 15;

        public InventoryReservationService(IUnitOfWork unitOfWork,ILogger<InventoryReservationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Dọn dẹp các đặt chỗ đã hết hạn
        /// </summary>
        /// <returns></returns>
        /*public async Task CleanupExpiredReservationsAsync()
        {
            var expiredReservations = await _unitOfWork.InventoryReservations.GetExpiredReservationsAsync();
            if (!expiredReservations.Any()) return;

            await _unitOfWork.InventoryReservations.DeleteRangeAsync(expiredReservations);

            await _unitOfWork.SaveChangesAsync();
        }*/


        /// <summary>
        ///     Trừ số lượng hàng đã đặt trước của người dùng sang kho chính và xóa các đặt chỗ đã xác nhận
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task<bool> ConfirmReservationAsync(Guid userId, string? transactionId = null, bool unboundOnly = false)
        {
            var userReservations = await _unitOfWork.InventoryReservations.GetActiveReservationsByUserAsync(userId, transactionId);
            if (unboundOnly)
            {
                // COD chỉ được ăn hold chưa gắn giao dịch ngân hàng, không được đụng hold VNPay đang bay
                userReservations = userReservations.Where(r => r.TransactionID == null).ToList();
            }
            if (!userReservations.Any()) return false;

            var productSizeIds = userReservations.Select(r => r.ProductSizeID).ToList();
            var productSizes = await _unitOfWork.ProductSizes.GetByIdsAsync(productSizeIds); 
            var productSizeMap = productSizes.ToDictionary(ps => ps.ProductSizeID);

            foreach (var reservation in userReservations)
            {
                if (productSizeMap.TryGetValue(reservation.ProductSizeID, out var productSize))
                {
                    productSize.Stock -= reservation.ReservedQuantity;
                }
                await _unitOfWork.InventoryReservations.DeleteAsync(reservation.ReservationID);
            }

            return true;
        }

        /// <summary>
        /// Kiểm tra xem kho và kho ảo có đủ hàng để đặt trước không
        /// </summary>
        /// <param name="cartItems"></param>
        /// <returns></returns>
        public async Task<bool> IsInventoryAvailableAsync(IEnumerable<CartItems> cartItems)
        {
            foreach (var item in cartItems)
            {
                var productSize = await _unitOfWork.ProductSizes.GetByIdAsync(item.ProductSizeID);
                if (productSize == null) return false;

                // kiểm tra tổng số lượng hàng đã đặt trước của nguời dùng 
                var reservedQuantity = await _unitOfWork.InventoryReservations.GetActiveReservedQuantityAsync(item.ProductSizeID);

                if (productSize.Stock - reservedQuantity < item.Quantity)
                    return false;
            }
            return true;
        }

        /// <summary>
        ///     giải phóng tất cả các đặt chỗ của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task<bool> ReleaseReservationAsync(Guid userId, string? transactionId = null)
        {
            var userReservations = await _unitOfWork.InventoryReservations.GetActiveReservationsByUserAsync(userId, transactionId);
            if (!userReservations.Any()) return true; 

            foreach (var reservation in userReservations)
            {
                await _unitOfWork.InventoryReservations.DeleteAsync(reservation.ReservationID);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Giải phóng hết lượng lần đặt trước của người dùng, thêm đặt chỗ cho người dùng vào kho ảo
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cartItems"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task<bool> ReserveInventoryAsync(Guid userId, IEnumerable<CartItems> cartItems, string? transactionId = null)
        {
            try
            {
                _logger.LogInformation("Cleaning up existing reservations for User {UserId} before creating new ones", userId);
                await ReleaseAllUserReservationsAsync(userId);

                if (!await IsInventoryAvailableAsync(cartItems))
                {
                    await _unitOfWork.RollbackAsync();
                    return false;
                }

                var reservationTime = DateTime.UtcNow;
                var expirationTime = reservationTime.AddMinutes(RESERVATION_MINUTES);
                var reservations = cartItems.Select(item => new InventoryReservations
                {
                    ProductSizeID = item.ProductSizeID,
                    UserID = userId,
                    ReservedQuantity = item.Quantity,
                    ReservationTime = reservationTime,
                    ExpirationTime = expirationTime,
                    TransactionID = transactionId
                }).ToList();

                await _unitOfWork.InventoryReservations.AddRangeAsync(reservations);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Giải phóng hold theo đúng mã giao dịch (dùng khi thanh toán thất bại / hủy đơn nháp).
        /// Không đụng tới hold của giao dịch khác.
        /// </summary>
        public async Task<bool> ReleaseByTransactionAsync(string transactionId)
        {
            var reservations = await _unitOfWork.InventoryReservations.GetByTransactionIdAsync(transactionId);
            if (!reservations.Any()) return true;

            await _unitOfWork.InventoryReservations.DeleteRangeAsync(reservations);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Don dẹp các đặt chỗ đã hết hạn (2 tầng):
        /// - Hold chưa gắn giao dịch: hết hạn là xóa.
        /// - Hold đã gắn giao dịch ngân hàng: giữ thêm grace period cho IPN trễ,
        ///   quá grace thì xóa + cancel đơn nháp Pending tương ứng.
        /// </summary>
        public async Task<int> DeleteExpiredReservationsAsync()
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;
                var expiredReservations = await _unitOfWork.InventoryReservations.GetExpiredReservationsAsync();

                var deletedCount = 0;
                var staleTransactionIds = new HashSet<string>();
                foreach (var reservation in expiredReservations)
                {
                    if (!string.IsNullOrEmpty(reservation.TransactionID)
                        && reservation.ExpirationTime.AddMinutes(RESERVATION_MINUTES) > now)
                    {
                        // Trong grace period: ngân hàng có thể callback trễ, giữ lại
                        continue;
                    }
                    if (!string.IsNullOrEmpty(reservation.TransactionID))
                    {
                        staleTransactionIds.Add(reservation.TransactionID);
                    }
                    await _unitOfWork.InventoryReservations.DeleteAsync(reservation.ReservationID);
                    deletedCount++;
                }

                // Cancel các đơn nháp Pending mà hold đã quá grace (tiền chưa về)
                foreach (var txnId in staleTransactionIds)
                {
                    var order = await _unitOfWork.Orders.GetByTransactionRefAsync(txnId);
                    if (order != null && order.Status == (int)OrderStatus.Pending)
                    {
                        await _unitOfWork.Orders.UpdateOrderStatus(order.OrderID, (int)OrderStatus.Cancelled);
                    }
                }

                if (deletedCount > 0)
                {
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
                }

                return deletedCount;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                return 0;
            }
        }

        /// <summary>
        /// Xóa các reservation CHƯA GẮN giao dịch của user (dùng khi reserve lại cho COD).
        /// Không đụng tới hold đã gắn mã giao dịch ngân hàng đang bay.
        /// </summary>
        public async Task<bool> ReleaseAllUserReservationsAsync(Guid userId)
        {
            try
            {
                var userReservations = await _unitOfWork.InventoryReservations.GetAllReservationsByUserIdAsync(userId);
                var unbound = userReservations.Where(r => r.TransactionID == null).ToList();

                if (!unbound.Any())
                {
                    _logger.LogDebug("No unbound reservations found for User {UserId}", userId);
                    return true;
                }

                _logger.LogInformation("Releasing {Count} unbound reservations for User {UserId}", unbound.Count, userId);

                await _unitOfWork.InventoryReservations.DeleteRangeAsync(unbound);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully released unbound reservations for User {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing reservations for User {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Thêm transactionId vào các đặt chỗ của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task<bool> AssignTransactionIdAsync(Guid userId, string transactionId)
        {
            var updatedCount = await _unitOfWork.InventoryReservations.AssignTransactionIdToUserReservationsAsync(userId, transactionId);
            return updatedCount > 0;
        }

        public async Task<bool> CreateReservationForCheckoutAsync(Guid userId, IEnumerable<CartItems> cartItems)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var success = await ReserveInventoryAsync(userId, cartItems);
                if (!success)
                {
                    await _unitOfWork.RollbackAsync();
                    return false;
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch {
                await _unitOfWork.RollbackAsync();
                return false;
            }
        }

        public async Task<CartValidationResultDTO> CheckAndSuggestCartInventoryAsync(IEnumerable<CartItems> currentCartItems)
        {
            var result = new CartValidationResultDTO();
            if (currentCartItems == null || !currentCartItems.Any())
            {
                return result;
            }
            var productSizeIds = currentCartItems.Select(ci => ci.ProductSizeID).Distinct().ToList();
            var productSizes = await _unitOfWork.ProductSizes.GetByIdsAsync(productSizeIds);
            var productSizeMap = productSizes.ToDictionary(ps => ps.ProductSizeID);
            var reservedQuantitiesMap = await _unitOfWork.InventoryReservations.GetActiveReservedQuantitiesAsync(productSizeIds);

            foreach (var item in currentCartItems)
            {
                int originalQuantity = item.Quantity;
                int adjustedQuantity = originalQuantity;
                int availableStock = 0;

                string productName = item.Products?.ProductName ?? "Sản phẩm không xác định";
                string productSizeName = item.ProductSizes?.Size ?? "N/A";

                if (productSizeMap.TryGetValue(item.ProductSizeID, out var productSize))
                {
                    reservedQuantitiesMap.TryGetValue(item.ProductSizeID, out int reservedQuantity);
                    availableStock = productSize.Stock - reservedQuantity;

                    if (originalQuantity > availableStock)
                    {
                        result.WasAdjusted = true;
                        adjustedQuantity = Math.Max(0, availableStock);

                        if (adjustedQuantity > 0)
                        {
                            result.Messages.Add($"Số lượng cho '{productName} - Size {productSizeName}' đã giảm còn {adjustedQuantity} do không đủ hàng.");
                        }
                        else
                        {
                            result.Messages.Add($"Sản phẩm '{productName} - Size {productSizeName}' đã hết hàng.");
                        }
                    }
                }
                else
                {
                    result.WasAdjusted = true;
                    adjustedQuantity = 0;
                    result.Messages.Add($"Sản phẩm '{productName}' không còn tồn tại.");
                }

                result.ValidatedItems.Add(new ValidatedCartItemDTO
                {
                    CartItemID = item.CartItemID,
                    ProductID = item.ProductID,
                    ProductName = item.Products.ProductName, 
                    ImageUrl = item.Products.ProductImages?.FirstOrDefault()?.ImageURL, 
                    ProductSizeID = item.ProductSizeID,
                    Size = item.ProductSizes.Size, 
                    Price = item.Products.Price,
                    OriginalQuantity = item.Quantity,
                    AvailableStock = availableStock,
                    AdjustedQuantity = adjustedQuantity
                });
            }

            return result;
        }
    }
}
