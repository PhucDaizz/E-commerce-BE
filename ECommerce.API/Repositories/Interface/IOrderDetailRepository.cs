using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.CartItem;
using ECommerce.API.Models.DTO.Order;

namespace ECommerce.API.Repositories.Interface
{
    public interface IOrderDetailRepository
    {
        Task<IEnumerable<OrderDetails>> CreateAsync(Guid orderID, IEnumerable<CartItemListDTO> cartItemListDTOs);
    }
}
