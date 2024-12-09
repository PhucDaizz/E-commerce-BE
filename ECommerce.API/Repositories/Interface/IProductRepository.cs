using ECommerce.API.Models.Domain;

namespace ECommerce.API.Repositories.Interface
{
    public interface IProductRepository
    {
        Task<Products> CreateAsync(Products products);
        Task<Products?> UpdateAsync(Products products);
        Task<Products?> DeleteAsync(int id);
        Task<Products?> GetByIdAsync(int id);
        Task<IEnumerable<Products>> GetAllAsync();

    }
}
