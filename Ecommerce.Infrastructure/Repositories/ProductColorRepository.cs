using AutoMapper;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Repositories
{
    public class ProductColorRepository : IProductColorRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductColorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductColors> CreateAsync(ProductColors productColors)
        {
            await _dbContext.ProductColors.AddAsync(productColors);
            await _dbContext.SaveChangesAsync();
            return productColors;
        }

        public async Task<ProductColors?> DeleteAsync(int id)
        {
            var existing = await _dbContext.ProductColors.FirstOrDefaultAsync(x => x.ProductColorID == id);
            if (existing == null)
            {
                return null;
            }
            _dbContext.ProductColors.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<ProductColors>> GetAllAsync()
        {
            var prductColors = await _dbContext.ProductColors.Include(x => x.ProductSizes).ToListAsync();
            return prductColors;
        }

        public async Task<ProductColors?> GetByIdAsync(int id)
        {
            var productColor = await _dbContext.ProductColors.Include(x => x.ProductSizes).FirstOrDefaultAsync(x => x.ProductColorID == id);
            if (productColor == null)
            {
                return null;
            }
            return productColor;
        }

        public async Task<ProductColors?> UpdateAsync(ProductColors productColors)
        {
            var existing = await _dbContext.ProductColors.FirstOrDefaultAsync(x => x.ProductColorID == productColors.ProductColorID);
            if (existing == null)
            {
                return null;
            }
            productColors.CreatedAt = existing.CreatedAt;
            _dbContext.ProductColors.Entry(existing).CurrentValues.SetValues(productColors);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<ProductColors>> GetProductColorSizeAsync(int productId)
        {
            var productColorSizes = await _dbContext.ProductColors.Include(x => x.ProductSizes)
                                        .Where(x => x.ProductID == productId)
                                        .ToListAsync();

            if (!productColorSizes.Any())
            {
                return null;
            }

            return productColorSizes;
        }

        public async Task<IEnumerable<ProductColors>> CreateRangeAsync(IEnumerable<ProductColors> productColors)
        {
            await _dbContext.ProductColors.AddRangeAsync(productColors);
            await _dbContext.SaveChangesAsync();
            return productColors;
        }

        public async Task<bool> DeleteProductColorSizeAsync(int productId)
        {
            var colors = await _dbContext.ProductColors
               .Include(x => x.ProductSizes)
               .Where(x => x.ProductID == productId)
               .ToListAsync();

            if (colors == null)
            {
                return false;
            }

            foreach (var color in colors)
            {
                _dbContext.ProductSizes.RemoveRange(color.ProductSizes);
                _dbContext.ProductColors.Remove(color);
            }

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
