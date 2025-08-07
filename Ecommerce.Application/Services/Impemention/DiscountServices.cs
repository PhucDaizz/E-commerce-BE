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
    public class DiscountServices : IDiscountServices
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountServices(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }
        public async Task<double> ApplyDiscountAsync(int discountId, Guid userID, double amount, bool isUpdateData = true)
        {
            var existing = await _discountRepository.GetByIdAsync(discountId);


            if (existing == null)
            {
                return amount;
            }

            if (!existing.IsActive)
            {
                return amount;
            }

            DateTime now = DateTime.Now;
            var timesUsedDiscount = await _discountRepository.GetUserUsageCountAsync(userID, discountId);
            if (timesUsedDiscount < existing.MaxUsagePerUser &&
                existing.StartDate <= now &&
                existing.EndDate >= now &&
                existing.Quantity > 0 &&
                amount >= existing.MinOrderValue)
            {
                switch (existing.DiscountType)
                {
                    case 1:  // Discount value
                        amount -= existing.DiscountValue;
                        break;
                    case 2: // Discount percent
                        amount -= amount * existing.DiscountValue / 100;
                        break;
                }

                if (isUpdateData)
                {
                    await _discountRepository.DecrementDiscountQuantityAsync(existing.DiscountID);
                }
                return amount;
            }
            return amount;
        }

        public async Task<Discounts> CanUseDiscount(Guid userId, string code, float amount)
        {
            var discount = await _discountRepository.GetDiscountByCodeAsync(code);
            if (discount == null)
            {
                return null;
            }
            if (!discount.IsActive || discount.MinOrderValue > amount)
            {
                return null;
            }

            var timesUsedDiscount = await _discountRepository.GetUserUsageCountAsync(userId, discount.DiscountID);
            if (timesUsedDiscount < discount.MaxUsagePerUser)
            {
                return discount;
            }
            return null;
        }
    }
}
