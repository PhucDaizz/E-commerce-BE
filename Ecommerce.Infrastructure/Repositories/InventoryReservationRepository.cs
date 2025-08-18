using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories
{
    public class InventoryReservationRepository : IInventoryReservationRepository
    {
        private readonly AppDbContext _dbContext;

        public InventoryReservationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task AddAsync(InventoryReservations reservation)
        {
            return _dbContext.InventoryReservations.AddAsync(reservation).AsTask();
        }

        public Task AddRangeAsync(IEnumerable<InventoryReservations> reservations)
        {
            return _dbContext.InventoryReservations.AddRangeAsync(reservations);
        }

        public async Task<int> AssignTransactionIdToUserReservationsAsync(Guid userId, string transactionId)
        {
            var now = DateTime.UtcNow;
            return await _dbContext.InventoryReservations
                .Where(r => r.UserID == userId && r.TransactionID == null && !r.IsExpired && r.ExpirationTime > now)
                .ExecuteUpdateAsync(updates => updates.SetProperty(r => r.TransactionID, transactionId));
        }

        public async Task<bool> DeleteAsync(int reservationID)
        {
            var reservation = await _dbContext.InventoryReservations
                .FirstOrDefaultAsync(r => r.ReservationID == reservationID);

            if (reservation == null)
                return false;

            _dbContext.InventoryReservations.Remove(reservation);
            return true;
        }

        public Task DeleteRangeAsync(IEnumerable<InventoryReservations> reservations)
        {
            _dbContext.InventoryReservations.RemoveRange(reservations);
            return Task.CompletedTask;
        }

        public async Task<List<InventoryReservations>> GetActiveReservationsByUserAsync(Guid userId, string? transactionId)
        {
            var query = _dbContext.InventoryReservations
                .Where(r => r.UserID == userId && r.ExpirationTime > DateTime.UtcNow);

            if (!string.IsNullOrEmpty(transactionId))
            {
                query = query.Where(r => r.TransactionID == transactionId);
            }

            return await query.ToListAsync();
        }

        public async Task<Dictionary<int, int>> GetActiveReservedQuantitiesAsync(IEnumerable<int> productSizeIds)
        {
            if (productSizeIds == null || !productSizeIds.Any())
            {
                return new Dictionary<int, int>();
            }

            var now = DateTime.UtcNow;
            return await _dbContext.InventoryReservations
                .Where(r => productSizeIds.Contains(r.ProductSizeID) && r.ExpirationTime > now && !r.IsExpired)
                .GroupBy(r => r.ProductSizeID)
                .Select(g => new {
                    ProductSizeId = g.Key,
                    TotalReserved = g.Sum(r => r.ReservedQuantity)
                })
                .ToDictionaryAsync(x => x.ProductSizeId, x => x.TotalReserved);
        }

        public async Task<int> GetActiveReservedQuantityAsync(int productSizeId)
        {
            var now = DateTime.UtcNow;
            return await _dbContext.InventoryReservations
                .Where(r => r.ProductSizeID == productSizeId && r.ExpirationTime > now && !r.IsExpired)
                .SumAsync(r => r.ReservedQuantity);
        }

        public async Task<List<InventoryReservations>> GetAllReservationsByUserIdAsync(Guid userId)
        {
            return await _dbContext.InventoryReservations
                .Where(r => r.UserID == userId)
                .ToListAsync();
        }

        public async Task<List<InventoryReservations>> GetExpiredReservationsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbContext.InventoryReservations
                .Where(r => r.ExpirationTime <= now)
                .ToListAsync();
        }

        public void UpdateRange(IEnumerable<InventoryReservations> reservations)
        {
            _dbContext.InventoryReservations.UpdateRange(reservations);
        }
    }
}
