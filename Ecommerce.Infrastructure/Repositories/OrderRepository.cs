using Ecommerce.Application.DTOS.Common;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _dbContext;

        public OrderRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Orders> CreateAsync(Orders order)
        {
            order.CreatedAt = DateTime.Now;
            order.UpdatedAt = DateTime.Now;
            await _dbContext.AddAsync(order);
            return order;
        }

        public async Task<Orders?> DeleteAsync(Guid id)
        {
            var existing = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == id);
            if (existing == null)
            {
                return null;
            }
            _dbContext.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<PagedResult<Orders>> GetAllAsync(Guid? userId, string? sortBy, bool isDESC = true, int page = 1, int itemInPage = 10)
        {
            var query = _dbContext.Orders.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(x => x.UserID == userId.Value).Include(x => x.Shippings);
            }

            switch (sortBy?.ToLower())
            {
                case "orderdate":
                    query = isDESC ? query.OrderByDescending(x => x.OrderDate) : query.OrderBy(x => x.OrderDate);
                    break;
                case "totalamount":
                    query = isDESC ? query.OrderByDescending(x => x.TotalAmount) : query.OrderBy(x => x.TotalAmount);
                    break;
                case "pending":
                    query = isDESC ? query.Where(x => x.Status == 0).OrderByDescending(x => x.OrderDate) : query.Where(x => x.Status == 0);
                    break;
                case "error":
                    query = isDESC ? query.Where(x => x.Status == 1).OrderByDescending(x => x.OrderDate) : query.Where(x => x.Status == 1);
                    break;
                case "completed":
                    query = isDESC ? query.Where(x => x.Status == 2).OrderByDescending(x => x.OrderDate) : query.Where(x => x.Status == 2);
                    break;
                case "cancel":
                    query = isDESC ? query.Where(x => x.Status == 3).OrderByDescending(x => x.OrderDate) : query.Where(x => x.Status == 3);
                    break;
                case "confirmed":
                    query = isDESC ? query.Where(x => x.Status == 4).OrderByDescending(x => x.OrderDate) : query.Where(x => x.Status == 4);
                    break;
                default:
                    query = isDESC ? query.OrderByDescending(x => x.OrderDate) : query.OrderBy(x => x.OrderDate);
                    break;
            }

            int totalCounts = await query.CountAsync();
            var orders = await query.Skip((page - 1) * itemInPage).Take(itemInPage).ToListAsync();
            int pageSize = totalCounts % itemInPage != 0 ? totalCounts / itemInPage + 1 : totalCounts / itemInPage;

            return new PagedResult<Orders>
            {
                Items = orders,
                TotalCount = totalCounts,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<Orders>?> GetAllByUserIdAsync(Guid userId)
        {
            var orders = await _dbContext.Orders.Where(o => o.UserID == userId).Include(x => x.Shippings).OrderByDescending(x => x.OrderDate).ToListAsync();
            if (!orders.Any() || orders == null)
            {
                return null;
            }
            return orders;
        }

        public async Task<Orders?> GetByIdAsync(Guid id, Guid? userId)
        {
            var existing = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == id && x.UserID == userId);
            return existing;
        }

        public async Task<Orders?> UpdateAsync(Orders order)
        {
            var existing = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == order.OrderID);
            if (existing == null)
            {
                return null;
            }
            order.UpdatedAt = DateTime.Now;
            _dbContext.Orders.Entry(existing).CurrentValues.SetValues(order);
            await _dbContext.SaveChangesAsync();
            return existing;
        }


        public async Task<Orders?> GetByIdAdminAsync(Guid id)
        {
            var existing = await _dbContext.Orders
                            .Include(x => x.Payments)
                            .Include(x => x.Shippings)
                            .Include(x => x.OrderDetails)
                                .ThenInclude(x => x.ProductSizes)
                                    .ThenInclude(x => x.ProductColors)
                                .ThenInclude(x =>  x.Products)
                                    .ThenInclude(pd => pd.ProductImages)
                            .FirstOrDefaultAsync(x => x.OrderID == id);
            if (existing == null)
            {
                return null;
            }
            return existing;
        }

        public async Task<Orders?> UpdateOrderStatus(Guid id, int status)
        {
            var existing = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == id);
            if (existing == null)
            {
                return null;
            }
            if (status >= 0 && status <= 4)
            {
                existing.Status = status;
                return existing;
            }
            return null;
        }

        public async Task<int> GetPurchaseCountAsync(Guid userId, int productId)
        {
            return await _dbContext.OrderDetails
                .CountAsync(od => od.ProductID == productId &&
                                   _dbContext.Orders
                                             .Any(o => o.OrderID == od.OrderID && o.UserID == userId));
        }
    }
}
