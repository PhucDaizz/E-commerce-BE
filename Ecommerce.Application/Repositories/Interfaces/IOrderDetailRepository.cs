using Ecommerce.Application.DTOS.CartItem;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IOrderDetailRepository
    {
        Task<IEnumerable<OrderDetails>> CreateAsync(Guid orderID, IEnumerable<CartItemListDTO> cartItemListDTOs);

        Task<IEnumerable<OrderDetails>> GetListOrderDetailsAsync(Guid orderID);
    }
}
