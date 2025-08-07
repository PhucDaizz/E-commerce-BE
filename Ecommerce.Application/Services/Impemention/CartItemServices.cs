using AutoMapper;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Impemention
{
    public class CartItemServices : ICartItemServices
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IMapper _mapper;
        private readonly IProductSizeRepository _productSizeRepository;
        private readonly IProductColorRepository _productColorRepository;

        public CartItemServices(ICartItemRepository cartItemRepository, IMapper mapper, IProductSizeRepository productSizeRepository, IProductColorRepository productColorRepository)
        {
            _cartItemRepository = cartItemRepository;
            _mapper = mapper;
            _productSizeRepository = productSizeRepository;
            _productColorRepository = productColorRepository;
        }

        public async Task<CartItems?> AddAsync(CartItems cartItems)
        {

            if (!await IsValidProductSizeAsync(cartItems.ProductID, cartItems.ProductSizeID))
            {
                return null;
            }

            var existingItem = await _cartItemRepository.FindByUserAndProductAndSizeAsync(cartItems.UserID, cartItems.ProductID, cartItems.ProductSizeID);

            if (existingItem == null)
            {
                if (cartItems.Quantity <= 0)
                {
                    return null;
                }
                else
                {
                    var result = await _cartItemRepository.CreateAsync(cartItems);
                    return result;
                }
            }
            else
            {
                existingItem.Quantity += cartItems.Quantity;

                if (existingItem.Quantity <= 0)
                {
                    await _cartItemRepository.DeleteAsync(existingItem);
                    return null;
                }
                else
                {
                    var result = await _cartItemRepository.UpdateAsync(existingItem);
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
            var existingItem = await _cartItemRepository.FindByUserAndCartItemIdAsync(cartItems.UserID, cartItems.CartItemID);

            if (existingItem == null)
            {
                return null;
            }

            existingItem.Quantity = cartItems.Quantity;

            if (existingItem.Quantity <= 0)
            {
                await _cartItemRepository.DeleteAsync(existingItem);
            }
            else
            {
                await _cartItemRepository.UpdateAsync(cartItems);
            }

            return existingItem.Quantity > 0 ? existingItem : null;
        }

        public async Task<bool> IsValidProductSizeAsync(int productId, int productSizeId)
        {
            var productColorSizes = await _productColorRepository.GetProductColorSizeAsync(productId);

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
