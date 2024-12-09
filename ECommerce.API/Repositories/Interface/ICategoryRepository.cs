using ECommerce.API.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Repositories.Interface
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
