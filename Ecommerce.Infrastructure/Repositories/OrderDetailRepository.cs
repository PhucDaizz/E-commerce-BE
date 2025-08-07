using Ecommerce.Application.DTOS.CartItem;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly AppDbContext _dbContext;

        public OrderDetailRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<OrderDetails>> CreateAsync(Guid orderID, IEnumerable<CartItemListDTO> cartItemListDTOs)
        {
            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            foreach (var cartItem in cartItemListDTOs)
            {
                var orderDetail = new OrderDetails
                {
                    OrderID = orderID,
                    ProductID = cartItem.ProductID,
                    ProductSizeId = cartItem.ProductSizeID,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.productDTO.Price,
                };
                _dbContext.OrderDetails.Add(orderDetail);
                orderDetailsList.Add(orderDetail);
            }
            await _dbContext.SaveChangesAsync();
            return orderDetailsList;
        }

        public async Task<IEnumerable<OrderDetails>> GetListOrderDetailsAsync(Guid orderID)
        {
            var orderDetail = await _dbContext.OrderDetails.Where(x => x.OrderID == orderID)
                                        .Include(x => x.ProductSizes)
                                            .ThenInclude(ps => ps.ProductColors)
                                                .ThenInclude(px => px.Products)
                                        .ToListAsync();
            if (orderDetail == null)
            {
                return null;
            }
            return orderDetail;
        }

        public async Task<bool> HasProductInAnyOrderAsync(int productId)
        {
            return await _dbContext.OrderDetails.AnyAsync(x => x.ProductID == productId);
        }
    }
}
