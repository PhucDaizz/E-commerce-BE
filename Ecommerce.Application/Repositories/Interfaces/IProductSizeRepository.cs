using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IProductSizeRepository
    {
        Task<ProductSizes> CreateAsync(ProductSizes productSizes);
        Task<ProductSizes?> UpdateAsync(ProductSizes productSizes);
        Task<ProductSizes?> DeleteAsync(int ProductSizeID);
        Task<ProductSizes?> GetByIdAsync(int ProductSizeID);
        Task<IEnumerable<ProductSizes>?> GetAllByColorAsync(int id);
        Task<bool> IsExistAsync(int ProductSizeID, string Size);
        Task<ProductSizes?> AddAsync(ProductSizes productSizes);
        Task<bool> DeleteByColorIDAsync(int colorID);
        Task<ProductSizes?> DeleteByColorAndSizeAsync(int colorID, string size);
        Task<bool> UpdateRangeAsync(IEnumerable<CartItems> cartItems);
    }
}
