using AutoMapper;
using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.CartItem;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ECommerce.API.Services.Impemention
{
    public class CartItemServices : ICartItemServices
    {
        private readonly AppDbContext dbContext;
        private readonly ICartItemRepository cartItemRepository;
        private readonly IMapper mapper;
        private readonly IProductSizeRepository productSizeRepository;
        private readonly IProductColorRepository productColorRepository;

        public CartItemServices(AppDbContext dbContext, ICartItemRepository cartItemRepository, IMapper mapper, IProductSizeRepository productSizeRepository, IProductColorRepository productColorRepository)
        {
            this.dbContext = dbContext;
            this.cartItemRepository = cartItemRepository;
            this.mapper = mapper;
            this.productSizeRepository = productSizeRepository;
            this.productColorRepository = productColorRepository;
        }

        public async Task<CartItems?> AddAsync(CartItems cartItems)
        {

            if (!await IsValidProductSizeAsync(cartItems.ProductID, cartItems.ProductSizeID))
            {
                return null; 
            }

            var existingItem = await dbContext.CartItems.FirstOrDefaultAsync(x => x.UserID == cartItems.UserID
                                        && x.ProductID == cartItems.ProductID
                                        && x.ProductSizeID == cartItems.ProductSizeID);

            if(existingItem == null)
            {
                if (cartItems.Quantity <= 0)
                {
                    return null;
                }
                else
                {
                    var result = await cartItemRepository.CreateAsync(cartItems);
                    return result;
                }
            }
            else
            {
                existingItem.Quantity += cartItems.Quantity;

                if(existingItem.Quantity <= 0)
                {
                    await cartItemRepository.DeleteAsync(existingItem);
                    return null;
                }
                else
                {
                    var result = await cartItemRepository.UpdateAsync(existingItem);
                    return result;
                }
            }
        }

        public async Task<CartItems?> UpdateAsync(CartItems cartItems)
        {
            if (!await IsValidProductSizeAsync(cartItems.ProductID, cartItems.ProductSizeID))
            {
                return null;
            }

            var existingItem = await dbContext.CartItems.FirstOrDefaultAsync(x => x.UserID == cartItems.UserID
                                        && x.CartItemID == cartItems.CartItemID);

            if (existingItem == null)
            {
                return null;
            }

            existingItem.Quantity = cartItems.Quantity;

            if (existingItem.Quantity <= 0)
            {
                await cartItemRepository.DeleteAsync(existingItem);
            }
            else
            {
                await cartItemRepository.UpdateAsync(cartItems);
            }

            return existingItem.Quantity > 0 ? existingItem : null;
        }

        public async Task<bool> IsValidProductSizeAsync(int productId, int productSizeId)
        {
            var productColorSizes = await productColorRepository.GetProductColorSizeAsync(productId);

            if (productColorSizes == null || !productColorSizes.Any())
            {
                return false;
            }

            var isValid = productColorSizes
                .SelectMany(pc => pc.ProductSizes) 
                .Any(ps => ps.ProductSizeID == productSizeId); 

            return isValid;
        }
    }
}
