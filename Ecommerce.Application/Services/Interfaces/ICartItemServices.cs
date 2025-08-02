using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface ICartItemServices
    {
        Task<CartItems?> AddAsync(CartItems cartItems);

        Task<CartItems?> UpdateAsync(CartItems cartItems);

        Task<bool> IsValidProductSizeAsync(int productId, int productSizeId);
    }
}
