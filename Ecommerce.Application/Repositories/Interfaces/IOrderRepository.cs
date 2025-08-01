using Ecommerce.Application.DTOS.Common;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Orders> CreateAsync(Orders order);
        Task<Orders?> UpdateAsync(Orders order);
        Task<Orders?> DeleteAsync(Guid id);
        Task<Orders?> GetByIdAsync(Guid id, Guid? userId);
        Task<IEnumerable<Orders>?> GetAllByUserIdAsync(Guid userId);
        Task<PagedResult<Orders>> GetAllAsync(Guid? userId, string? sortBy, bool isDESC = true, int page = 1, int itemInPage = 10);
        Task<Orders?> GetByIdAdminAsync(Guid id);
        Task<Orders?> UpdateOrderStatus(Guid id, int status);
    }
}
