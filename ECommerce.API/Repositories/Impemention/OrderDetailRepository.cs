using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.CartItem;
using ECommerce.API.Models.DTO.Order;
using ECommerce.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Impemention
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly AppDbContext dbContext;

        public OrderDetailRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
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
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.productDTO.Price
                };
                dbContext.OrderDetails.Add(orderDetail);
                orderDetailsList.Add(orderDetail);
            }
            await dbContext.SaveChangesAsync();
            return orderDetailsList;
        }

        public async Task<IEnumerable<OrderDetails>> GetListOrderDetailsAsync(Guid orderID)
        {
            var orderDetail = await dbContext.OrderDetails.Where(x => x.OrderID == orderID).Include(x => x.Products).ToListAsync();
            if (orderDetail == null)
            {
                return null;
            }
            return orderDetail;
        }
    }
}
