using Ecommerce.Application.DTOS.Common;
using Ecommerce.Application.DTOS.Product;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
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
