using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface ICartItemRepository
    {
        Task<CartItems> CreateAsync(CartItems cartItems);
        Task<CartItems?> UpdateAsync(CartItems cartItems);
        Task<CartItems?> DeleteAsync(CartItems cartItems);
        Task<IEnumerable<CartItems>?> GetAllAsync(Guid UserID);
        Task<bool> DeleteAllByUserIDAsync(Guid UserID);
        Task<bool> ClearAllByProductIDAsync(int ProductID);
    }
}
