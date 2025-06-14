using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Product;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerce.API.Repositories.Interface
{
    public interface IProductRepository
    {
        Task<Products> CreateAsync(Products products);
        Task<Products?> UpdateAsync(Products products);
        Task<Products?> DeleteAsync(int id);
        Task<Products?> GetByIdAsync(int id);
        Task<Products?> GetDetailAsync(int id);
        Task<PagedResult<ListProductDTO>> GetAllAsync(string? productName, bool isDESC = false, int page = 1, int itemInPage = 20, string sortBy = "CreatedAt", int? categoryId = null);
        Task<bool> ExistsAsync(int id);
        Task<PagedResult<ListProductAdminDTO>> GetAllAdminAsync(string? productName, bool isDESC = false, int page = 1, int itemInPage = 20, string sortBy = "CreatedAt", int? categoryId = null);
        Task<bool> ToPublicAync(int id);
    
    
    }
}
