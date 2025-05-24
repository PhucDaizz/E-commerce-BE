using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services.Impemention
{
    public class DiscountServices : IDiscountServices
    {
        private readonly IDiscountRepository discountRepository;
        private readonly AppDbContext dbContext;

        public DiscountServices(IDiscountRepository discountRepository, AppDbContext dbContext)
        {
            this.discountRepository = discountRepository;
            this.dbContext = dbContext;
        }
        public async Task<double> ApplyDiscountAsync(int discountId, Guid userID, double amount, bool isUpdateData = true)
        {
            var existing = await dbContext.Discounts.FirstOrDefaultAsync(x => x.DiscountID == discountId);

            if (existing == null)
            {
                return amount;
            }
            
            if(!existing.IsActive)
            {
                return amount;
            }
                
            DateTime now = DateTime.Now;
            var timesUsedDiscount = await dbContext.Orders.Where(x => x.UserID == userID && x.DiscountID == discountId).CountAsync();
            if (timesUsedDiscount < existing.MaxUsagePerUser && 
                existing.StartDate <= now && 
                existing.EndDate >= now && 
                existing.Quantity > 0 && 
                amount >= existing.MinOrderValue)
            {
                switch(existing.DiscountType)
                {
                    case 1:  // Discount value
                        amount -= existing.DiscountValue;
                        break;
                    case 2: // Discount percent
                        amount -= amount * existing.DiscountValue / 100;
                        break;
                }

                existing.Quantity -= 1;


                if (existing.Quantity == 0)
                {
                    existing.IsActive = false;
                }

                if (isUpdateData)
                {
                    dbContext.Discounts.Update(existing);
                    await dbContext.SaveChangesAsync();
                }
                return amount;
            }
            return amount;
        }

        public async Task<Discounts> CanUseDiscount(Guid userId, string code, float amount)
        {
            var discount = await discountRepository.GetDiscountByCodeAsync(code);
            if (discount == null)
            {
                return null;
            }
            if(!discount.IsActive || discount.MinOrderValue > amount)
            {
                return null;
            }
            var timesUsedDiscount = await dbContext.Orders.Where(x => x.UserID == userId && x.DiscountID == discount.DiscountID).CountAsync();
            if (timesUsedDiscount < discount.MaxUsagePerUser)
            {
                return discount;
            }
            return null;
        }
    }
}
