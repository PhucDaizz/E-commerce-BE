using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Product;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Repositories.Interface
{
    public interface IOrderRepository
    {
        Task<Orders> CreateAsync(Orders order);
        Task<Orders?> UpdateAsync(Orders order);
        Task<Orders?> DeleteAsync(Guid id);
        Task<Orders?> GetByIdAsync(Guid id, Guid? userId);
        Task<IEnumerable<Orders>?> GetAllByUserIdAsync(Guid userId);
        Task<PagedResult<Orders>> GetAllAsync([FromQuery] Guid? userId, [FromQuery] string? sortBy, [FromQuery] bool isDESC = true, [FromQuery]int page = 1, [FromQuery]int itemInPage = 10);
        Task<Orders?> GetByIdAdminAsync(Guid id);
    }
}
