using AutoMapper;
using Ecommerce.Application.DTOS.Common;
using Ecommerce.Application.DTOS.Product;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace Ecommerce.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<Products> CreateAsync(Products products)
        {
            try
            {
                products.ProductNameUnsigned = RemoveDiacritics(products.ProductName);
                await _dbContext.Products.AddAsync(products);
                await _dbContext.SaveChangesAsync();
                return products;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message); // Xem thông báo lỗi chi tiết
                throw;
            }
        }

        public async Task<Products?> DeleteAsync(int id)
        {
            var existing = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == id);
            if (existing == null)
            {
                return null;
            }
            _dbContext.Products.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<PagedResult<ListProductDTO>> GetAllAsync(string? productName, bool isDESC = false, int page = 1, int itemInPage = 20, string sortBy = "CreatedAt", int? categoryId = null)
        {
            var query = _dbContext.Products.AsQueryable();
            if (!string.IsNullOrEmpty(productName))
            {
                var unsignedKeyword = RemoveDiacritics(productName.ToLower());

                query = query.Where(p =>
                    p.ProductName.ToLower().Contains(productName.ToLower()) ||
                    p.ProductNameUnsigned.ToLower().Contains(unsignedKeyword));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryID == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "price":
                        query = isDESC ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price);
                        break;
                    case "name":
                        query = isDESC ? query.OrderByDescending(x => x.ProductName) : query.OrderBy(x => x.ProductName);
                        break;
                    default:
                        query = isDESC ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt);
                        break;
                }
            }

            query = query.Where(x => x.IsPublic == true);
            int totalCounts = await query.CountAsync();
            var products = await query.Skip((page - 1) * itemInPage).Take(itemInPage).Include(x => x.ProductImages).ToListAsync();
            int pageSize = totalCounts % itemInPage != 0 ? totalCounts / itemInPage + 1 : totalCounts / itemInPage;

            var productsDTO = _mapper.Map<List<ListProductDTO>>(products);

            return new PagedResult<ListProductDTO>
            {
                Items = productsDTO,
                TotalCount = totalCounts,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<Products?> GetByIdAsync(int id)
        {
            var existing = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == id);
            if (existing == null)
            {
                return null;
            }

            return existing;
        }

        public async Task<Products?> UpdateAsync(Products products)
        {
            var existing = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == products.ProductID);
            if (existing == null)
            {
                return null;
            }
            products.CreatedAt = existing.CreatedAt;
            _dbContext.Products.Entry(existing).CurrentValues.SetValues(products);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.Products.AnyAsync(x => x.ProductID == id);
        }

        public async Task<PagedResult<ListProductAdminDTO>> GetAllAdminAsync(string? productName, bool isDESC = false, int page = 1, int itemInPage = 20, string sortBy = "CreatedAt", int? categoryId = null)
        {
            var query = _dbContext.Products
                .Include(x => x.ProductColors)
                    .ThenInclude(x => x.ProductSizes)
                .AsQueryable();


            if (!string.IsNullOrEmpty(productName))
            {
                var unsignedKeyword = RemoveDiacritics(productName.ToLower());

                query = query.Where(p =>
                    p.ProductName.ToLower().Contains(productName.ToLower()) ||
                    p.ProductNameUnsigned.ToLower().Contains(unsignedKeyword));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryID == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "price":
                        query = isDESC ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price);
                        break;
                    case "name":
                        query = isDESC ? query.OrderByDescending(x => x.ProductName) : query.OrderBy(x => x.ProductName);
                        break;
                    default:
                        query = isDESC ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt);
                        break;
                }
            }

            int totalCounts = await query.CountAsync();
            var products = await query.Skip((page - 1) * itemInPage).Take(itemInPage).Include(x => x.ProductImages).ToListAsync();
            int pageSize = totalCounts % itemInPage != 0 ? totalCounts / itemInPage + 1 : totalCounts / itemInPage;

            var productsDTO = _mapper.Map<List<ListProductAdminDTO>>(products);

            return new PagedResult<ListProductAdminDTO>
            {
                Items = productsDTO,
                TotalCount = totalCounts,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<bool> ToPublicAync(int id)
        {
            var existing = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == id);
            if (existing == null)
            {
                return false;
            }

            existing.IsPublic = !existing.IsPublic;

            _dbContext.Entry(existing).CurrentValues.SetValues(existing);
            return true;
        }

        public async Task<Products?> GetDetailAsync(int id)
        {
            var product = await _dbContext.Products
                .Include(x => x.ProductImages)
                .Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.ProductID == id);
            return product;
        }

        private string RemoveDiacritics(string productName)
        {
            var normalized = productName.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC).ToLower();
        }
    }
}
