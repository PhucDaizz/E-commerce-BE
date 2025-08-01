using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Categories> CreateAsync(Categories categories);
        Task<Categories?> UpdateAsync(Categories categories);
        Task<Categories?> DeleteAsync(int id);
        Task<Categories?> GetByIdAsync(int id);
        Task<IEnumerable<Categories>> GetAllAsync();
    }
}
