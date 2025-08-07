using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IProductColorRepository
    {
        Task<ProductColors?> GetByIdAsync(int id);
        Task<ProductColors> CreateAsync(ProductColors productColors);
        Task<IEnumerable<ProductColors>> CreateRangeAsync(IEnumerable<ProductColors> productColors);
        Task<ProductColors?> UpdateAsync(ProductColors productColors);
        Task<ProductColors?> DeleteAsync(int id);
        Task<IEnumerable<ProductColors>> GetAllAsync();
        Task<IEnumerable<ProductColors>> GetProductColorSizeAsync(int productId);
        Task<bool> DeleteProductColorSizeAsync(int productId);

    }
}
