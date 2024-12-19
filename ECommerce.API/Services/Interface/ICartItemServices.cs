using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.CartItem;

namespace ECommerce.API.Services.Interface
{
    public interface ICartItemServices
    {
        Task<CartItems?> AddAsync(CartItems cartItems);

        Task<CartItems?> UpdateAsync(CartItems cartItems);

        Task<bool> IsValidProductSizeAsync(int productId, int productSizeId);
    }
}
