using Ecommerce.Application.DTOS.Discount;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        Task<Discounts> CreateAsync(Discounts discounts);
        Task<Discounts?> GetByIdAsync(int id);
        Task<ListDiscountDTO?> GetAllAsync(int page = 1, int itemsInPage = 20, string sortBy = "IsActive", bool isDESC = false);
        Task<Discounts?> UpdateAsync(Discounts discounts);
        Task<Discounts?> DeleteAsync(int id);
        Task<Discounts?> GetDiscountByCodeAsync(string code);
        Task<Discounts?> ActiveAsync(int discountId);
        Task<int> GetUserUsageCountAsync(Guid userId, int discountId);
        Task<bool> DecrementDiscountQuantityAsync(int discountId);
    }
}
