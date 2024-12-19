using ECommerce.API.Models.Domain;

namespace ECommerce.API.Repositories.Interface
{
    public interface ICartItemRepository
    {
        Task<CartItems> CreateAsync(CartItems cartItems);
        Task<CartItems?> UpdateAsync(CartItems cartItems);
        Task<CartItems?> DeleteAsync(CartItems cartItems);
        Task<IEnumerable<CartItems>?> GetAllAsync(Guid UserID);
        Task<bool> DeleteAllByUserIDAsync(Guid UserID);
    }
}
