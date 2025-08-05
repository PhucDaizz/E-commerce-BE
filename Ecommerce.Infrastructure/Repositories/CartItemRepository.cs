using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _dbContext;

        public CartItemRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ClearAllByProductIDAsync(int ProductID)
        {
            var products = await _dbContext.CartItems.Where(x => x.ProductID == ProductID).ToListAsync();
            if (products.Any())
            {
                _dbContext.CartItems.RemoveRange(products);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<CartItems> CreateAsync(CartItems cartItems)
        {
            await _dbContext.CartItems.AddAsync(cartItems);
            await _dbContext.SaveChangesAsync();
            return cartItems;
        }

        public async Task<bool> DeleteAllByUserIDAsync(Guid UserID)
        {
            var cartItems = await _dbContext.CartItems.Where(x => x.UserID == UserID).ToListAsync();

            if (cartItems.Any())
            {
                _dbContext.CartItems.RemoveRange(cartItems);
                return true;
            }
            return false;
        }

        public async Task<CartItems?> DeleteAsync(CartItems cartItems)
        {
            var existing = await _dbContext.CartItems.FirstOrDefaultAsync(x => x.CartItemID == cartItems.CartItemID);
            if (existing == null)
            {
                return null;
            }
            _dbContext.CartItems.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<CartItems?> FindByUserAndCartItemIdAsync(Guid userId, int cartItemId)
        {
            return await _dbContext.CartItems.FirstOrDefaultAsync(ci =>
                   ci.UserID == userId &&
                   ci.CartItemID == cartItemId);
        }

        public async Task<CartItems?> FindByUserAndProductAndSizeAsync(Guid userId, int productId, int productSizeId)
        {
            return await _dbContext.CartItems.FirstOrDefaultAsync(ci =>
                    ci.UserID == userId &&
                    ci.ProductID == productId &&
                    ci.ProductSizeID == productSizeId);
        }

        public async Task<IEnumerable<CartItems>?> GetAllAsync(Guid UserID)
        {
            var existing = await _dbContext.CartItems
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
            var exsting = await _dbContext.CartItems.FirstOrDefaultAsync(x => x.CartItemID == cartItems.CartItemID);
            if (exsting == null)
            {
                return null;
            }
            _dbContext.CartItems.Entry(exsting).CurrentValues.SetValues(cartItems);
            await _dbContext.SaveChangesAsync();
            return exsting;
        }
    }
}
