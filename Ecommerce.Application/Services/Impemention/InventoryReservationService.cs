using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
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
        public async Task<bool> ConfirmReservationAsync(Guid userId, string? transactionId = null)
        {
            var userReservations = await _unitOfWork.InventoryReservations.GetActiveReservationsByUserAsync(userId, transactionId);
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
        /// Giải phóng hết lượng lần đạt trước của người dung, thêm đặt chỗ cho người dùng vào kho ảo
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
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Don dẹp các đặt chỗ đã hết hạn
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteExpiredReservationsAsync()
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var currentTime = DateTime.Now;
                var expiredReservations = await _unitOfWork.InventoryReservations.GetExpiredReservationsAsync();

                var deletedCount = 0;
                foreach (var reservation in expiredReservations)
                {
                    await _unitOfWork.InventoryReservations.DeleteAsync(reservation.ReservationID);
                    deletedCount++;
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
        /// Xóa TẤT CẢ reservation của user (bao gồm cả chưa hết hạn)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> ReleaseAllUserReservationsAsync(Guid userId)
        {
            try
            {
                var userReservations = await _unitOfWork.InventoryReservations.GetAllReservationsByUserIdAsync(userId);

                if (!userReservations.Any())
                {
                    _logger.LogDebug("No existing reservations found for User {UserId}", userId);
                    return true;
                }

                _logger.LogInformation("Releasing {Count} existing reservations for User {UserId}", userReservations.Count, userId);

                await _unitOfWork.InventoryReservations.DeleteRangeAsync(userReservations);

                _logger.LogInformation("Successfully released reservations for User {UserId}", userId);
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
    }
}
