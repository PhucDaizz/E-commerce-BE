using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Impemention
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext dbContext;

        public CartItemRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> ClearAllByProductIDAsync(int ProductID)
        {
            var products = await dbContext.CartItems.Where(x => x.ProductID == ProductID).ToListAsync();
            if (products.Any())
            {
                dbContext.CartItems.RemoveRange(products);
                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;    
        }

        public async Task<CartItems> CreateAsync(CartItems cartItems)
        {
            await dbContext.CartItems.AddAsync(cartItems);
            await dbContext.SaveChangesAsync();
            return cartItems;
        }

        public async Task<bool> DeleteAllByUserIDAsync(Guid UserID)
        {
            var cartItems = await dbContext.CartItems.Where(x => x.UserID == UserID).ToListAsync();

            if (cartItems.Any()) { 
                dbContext.CartItems.RemoveRange(cartItems); 
                await dbContext.SaveChangesAsync(); 
                return true; 
            }
            return false;
        }

        public async Task<CartItems?> DeleteAsync(CartItems cartItems)
        {
            var existing = await dbContext.CartItems.FirstOrDefaultAsync(x => x.CartItemID == cartItems.CartItemID);
            if (existing == null)
            {
                return null;
            }
            dbContext.CartItems.Remove(existing);
            await dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<CartItems>?> GetAllAsync(Guid UserID)
        {
            var existing = await dbContext.CartItems
                            .Where(x => x.UserID == UserID)
                            .Include(x => x.ProductSizes)
                            .Include(x => x.Products)
                                .ThenInclude(p => p.ProductImages)
                            .OrderBy(x => x.CreatedAt).ToListAsync();
            if (existing == null)
            {
                return null;
            }
            return existing;
        }

        public async Task<CartItems?> UpdateAsync(CartItems cartItems)
        {
            var exsting = await dbContext.CartItems.FirstOrDefaultAsync(x => x.CartItemID == cartItems.CartItemID);
            if (exsting == null)
            {
                return null;
            }
            dbContext.CartItems.Entry(exsting).CurrentValues.SetValues(cartItems);
            await dbContext.SaveChangesAsync();
            return exsting;
        }
    }
}
