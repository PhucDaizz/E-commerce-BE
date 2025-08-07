using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IProductImageRepository
    {
        Task<ProductImages> CreateAsync(ProductImages productImages);
        Task<ProductImages> GetByIdAsync(int id);
        Task<ProductImages?> UpdateAsync(ProductImages productImages);
        Task<ProductImages?> DeleteAsync(int id);
        Task<IEnumerable<ProductImages>> GetAllByProductIDAsync(int productId);
        Task<IEnumerable<ProductImages>> CreateImagesAsync(IEnumerable<ProductImages> imagesList);
        Task<int> CountByProductIdAsync(int id);
    }
}
