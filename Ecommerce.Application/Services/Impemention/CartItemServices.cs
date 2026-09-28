using Ecommerce.Application.DTOS.CartItem;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Repositories.Persistence;
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
        private readonly IProductColorRepository _productColorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CartItemServices(ICartItemRepository cartItemRepository, IProductColorRepository productColorRepository, IUnitOfWork unitOfWork)
        {
            _cartItemRepository = cartItemRepository;
            _productColorRepository = productColorRepository;
            _unitOfWork = unitOfWork;
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

        public async Task<bool> MergeCartAsync(Guid userId, List<CreateCartItemDTO> localCartItems)
        {
            if (localCartItems == null || !localCartItems.Any())
                return true;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var existingCart = (await _unitOfWork.CartItems.GetAllAsync(userId)).ToList();

                var existingItemsDict = existingCart
                    .GroupBy(x => new { x.ProductID, x.ProductSizeID })
                    .ToDictionary(g => g.Key, g => g.First());

                foreach (var localItem in localCartItems)
                {
                    if (!await IsValidProductSizeAsync(localItem.ProductID, localItem.ProductSizeID))
                        continue;

                    if (localItem.Quantity <= 0)
                        continue;

                    var key = new { localItem.ProductID, localItem.ProductSizeID };

                    if (existingItemsDict.TryGetValue(key, out var existingItem))
                    {
                        existingItem.Quantity += localItem.Quantity;
                        existingItem.UpdatedAt = DateTime.Now;
                        await _unitOfWork.CartItems.UpdateAsync(existingItem); 
                    }
                    else
                    {
                        var newCartItem = new CartItems
                        {
                            UserID = userId,
                            ProductID = localItem.ProductID,
                            Quantity = localItem.Quantity,
                            ProductSizeID = localItem.ProductSizeID,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        await _unitOfWork.CartItems.CreateAsync(newCartItem);
                    }
                }

                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
