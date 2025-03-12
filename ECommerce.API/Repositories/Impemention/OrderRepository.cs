using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Product;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Impemention
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ECommerceDbContext dbContext;

        public OrderRepository(ECommerceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Orders> CreateAsync(Orders order)
        {
            order.CreatedAt = DateTime.Now;
            order.UpdatedAt = DateTime.Now;
            await dbContext.AddAsync(order);
            await dbContext.SaveChangesAsync();
            return order;
        }

        public async Task<Orders?> DeleteAsync(Guid id)
        {
            var existing = await dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == id);
            if (existing == null)
            {
                return null;
            }
            dbContext.Remove(existing);
            await dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<PagedResult<Orders>> GetAllAsync([FromQuery] Guid? userId, [FromQuery] string? sortBy, [FromQuery] bool isDESC = true, [FromQuery] int page = 1, [FromQuery] int itemInPage = 10)
        {
            var query = dbContext.Orders.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(x => x.UserID == userId.Value);
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
            var orders = await dbContext.Orders.Where(o => o.UserID == userId).OrderByDescending(x => x.OrderDate).ToListAsync();
            if (!orders.Any() || orders == null)
            {
                return null;
            }
            return orders;
        }

        public async Task<Orders?> GetByIdAsync(Guid id, Guid? userId)
        {
            var existing = await dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == id && x.UserID == userId);
            return existing;
        }

        public async Task<Orders?> UpdateAsync(Orders order)
        {
            var existing = await dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == order.OrderID);
            if (existing == null)
            {
                return null;
            }
            order.UpdatedAt = DateTime.Now;
            dbContext.Orders.Entry(existing).CurrentValues.SetValues(order);
            await dbContext.SaveChangesAsync();
            return existing;
        }


        public async Task<Orders?> GetByIdAdminAsync(Guid id)
        {
            var existing = await dbContext.Orders
                            .Include(x => x.Payments)
                            .Include(x => x.Shippings)
                            .Include(x => x.OrderDetails)
                                .ThenInclude(x => x.Products)
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
            var existing = await dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == id);
            if(existing == null)
            {
                return null;
            }
            if(status >= 0 && status <= 4 && status != 1 )
            {
                existing.Status = status;
                dbContext.Entry(existing).CurrentValues.SetValues(existing);
                await dbContext.SaveChangesAsync(); 
                return existing;
            }
            return null;
        }
    }
}
