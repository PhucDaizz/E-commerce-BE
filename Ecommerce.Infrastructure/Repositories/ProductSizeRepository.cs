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
    public class ProductSizeRepository: IProductSizeRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductSizeRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ProductSizes> CreateAsync(ProductSizes productSizes)
        {
            await _dbContext.ProductSizes.AddAsync(productSizes);
            await _dbContext.SaveChangesAsync();
            return productSizes;
        }

        public async Task<ProductSizes?> DeleteAsync(int ProductSizeID)
        {
            var existing = await _dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductSizeID == ProductSizeID);
            if (existing == null)
            {
                return null;
            }
            _dbContext.ProductSizes.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<ProductSizes?> GetByIdAsync(int ProductSizeID)
        {
            var existing = await _dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductSizeID == ProductSizeID);
            if (existing == null)
            {
                return null;
            }
            return existing;
        }

        public async Task<ProductSizes?> UpdateAsync(ProductSizes productSizes)
        {
            var existing = await _dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductSizeID == productSizes.ProductSizeID);
            if (existing == null)
            {
                return null;
            }
            productSizes.UpdatedAt = DateTime.Now;
            _dbContext.ProductSizes.Entry(existing).CurrentValues.SetValues(productSizes);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<ProductSizes>?> GetAllByColorAsync(int id)
        {
            var productSizes = await _dbContext.ProductSizes.Where(x => x.ProductColorID == id).ToListAsync();
            if (!productSizes.Any())
            {
                return null;
            }
            return productSizes;
        }

        public async Task<bool> IsExistAsync(int productColorID, string size)
        {
            var existing = await _dbContext.ProductSizes
                .FirstOrDefaultAsync(x => x.ProductColorID == productColorID && x.Size == size);

            return existing != null;
        }


        public async Task<ProductSizes?> AddAsync(ProductSizes productSizes)
        {
            var existing = await _dbContext.ProductSizes
                .FirstOrDefaultAsync(x => x.ProductColorID == productSizes.ProductColorID && x.Size == productSizes.Size);

            if (existing != null)
            {
                existing.Stock += productSizes.Stock;
                existing.UpdatedAt = DateTime.Now;

                _dbContext.ProductSizes.Update(existing);
                await _dbContext.SaveChangesAsync();

                return existing;
            }

            return null;
        }

        public async Task<bool> DeleteByColorIDAsync(int colorID)
        {
            var existing = _dbContext.ProductSizes.Where(x => x.ProductColorID == colorID);
            if (!existing.Any())
            {
                return false;
            }
            _dbContext.ProductSizes.RemoveRange(existing);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ProductSizes?> DeleteByColorAndSizeAsync(int colorID, string size)
        {
            if (string.IsNullOrEmpty(size) || colorID == null)
            {
                return null;
            }
            var existing = await _dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductColorID == colorID && x.Size == size);
            if (existing == null)
            {
                return null;
            }
            _dbContext.ProductSizes.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> UpdateRangeAsync(IEnumerable<CartItems> cartItems)
        {
            List<ProductSizes> productSizesUpdate = new List<ProductSizes>();
            foreach (var item in cartItems)
            {
                var productSize = await _dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductSizeID == item.ProductSizeID);
                if (productSize == null)
                {
                    return false;
                }
                productSize.Stock -= item.Quantity;
                productSizesUpdate.Add(productSize);
            }
            _dbContext.ProductSizes.UpdateRange(productSizesUpdate);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
