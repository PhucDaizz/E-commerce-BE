using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Discount;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Repositories.Interface
{
    public interface IDiscountRepository
    {
        Task<Discounts> CreateAsync(Discounts discounts);
        Task<Discounts?> GetByIdAsync(int id);
        Task<ListDiscountDTO?> GetAllAsync([FromQuery] int page = 1, [FromQuery] int itemsInPage = 20, [FromQuery] string sortBy = "IsActive", [FromQuery] bool isDESC = false);
        Task<Discounts?> UpdateAsync(Discounts discounts);
        Task<Discounts?> DeleteAsync(int id);
        Task<Discounts?> GetDiscountByCodeAsync(string code);
        Task<Discounts?> ActiveAsync(int discountId);
    }
}
