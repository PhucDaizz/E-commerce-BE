using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IInventoryReservationRepository
    {
        Task AddAsync(InventoryReservations reservation);

        Task AddRangeAsync(IEnumerable<InventoryReservations> reservations);

        Task<int> GetActiveReservedQuantityAsync(int productSizeId);

        Task<List<InventoryReservations>> GetActiveReservationsByUserAsync(Guid userId, string? transactionId);

        Task<List<InventoryReservations>> GetAllReservationsByUserIdAsync(Guid userId);

        Task<List<InventoryReservations>> GetExpiredReservationsAsync();
        Task<int> AssignTransactionIdToUserReservationsAsync(Guid userId, string transactionId);

        void UpdateRange(IEnumerable<InventoryReservations> reservations);
        Task<bool> DeleteAsync(int reservationID);
        Task DeleteRangeAsync(IEnumerable<InventoryReservations> reservations);
        Task<Dictionary<int, int>> GetActiveReservedQuantitiesAsync(IEnumerable<int> productSizeIds);
    }
}
