using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface IInventoryReservationService
    {
        Task<bool> ReserveInventoryAsync(Guid userId, IEnumerable<CartItems> cartItems, string? transactionId = null);
        Task<bool> ConfirmReservationAsync(Guid userId, string? transactionId = null);
        Task<bool> ReleaseReservationAsync(Guid userId, string? transactionId = null);
        Task<bool> IsInventoryAvailableAsync(IEnumerable<CartItems> cartItems);
        Task<bool> AssignTransactionIdAsync(Guid userId, string transactionId);
        //Task CleanupExpiredReservationsAsync();
        Task<int> DeleteExpiredReservationsAsync();
        Task<bool> ReleaseAllUserReservationsAsync(Guid userId);
        Task<bool> CreateReservationForCheckoutAsync(Guid userId, IEnumerable<CartItems> cartItems);
    }
}
