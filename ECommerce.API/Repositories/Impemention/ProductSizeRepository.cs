using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Impemention
{
    public class ProductSizeRepository : IProductSizeRepository
    {
        private readonly ECommerceDbContext dbContext;

        public ProductSizeRepository(ECommerceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<ProductSizes> CreateAsync(ProductSizes productSizes)
        {
            await dbContext.ProductSizes.AddAsync(productSizes);
            await dbContext.SaveChangesAsync();
            return productSizes;
        }

        public async Task<ProductSizes?> DeleteAsync(int ProductSizeID)
        {
            var existing = await dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductSizeID == ProductSizeID);
            if (existing == null)
            {
                return null;
            }
            dbContext.ProductSizes.Remove(existing);
            await dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<ProductSizes?> GetByIdAsync(int ProductSizeID)
        {
            var existing = await dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductSizeID == ProductSizeID); 
            if (existing == null)
            {
                return null;
            }
            return existing;
        }

        public async Task<ProductSizes?> UpdateAsync(ProductSizes productSizes)
        {
            var existing = await dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductSizeID == productSizes.ProductSizeID);
            if (existing == null)
            {
                return null;
            }
            productSizes.UpdatedAt = DateTime.Now;
            dbContext.ProductSizes.Entry(existing).CurrentValues.SetValues(productSizes);
            await dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<ProductSizes>?> GetAllByColorAsync(int id)
        {
            var productSizes = await dbContext.ProductSizes.Where(x => x.ProductColorID == id).ToListAsync();
            if (!productSizes.Any())
            {
                return null;
            }
            return productSizes;
        }

        public async Task<bool> IsExistAsync(int productColorID, string size)
        {
            var existing = await dbContext.ProductSizes
                .FirstOrDefaultAsync(x => x.ProductColorID == productColorID && x.Size == size);

            return existing != null;
        }


        public async Task<ProductSizes?> AddAsync(ProductSizes productSizes)
        {
            var existing = await dbContext.ProductSizes
                .FirstOrDefaultAsync(x => x.ProductColorID == productSizes.ProductColorID && x.Size == productSizes.Size);

            if (existing != null)
            {
                existing.Stock += productSizes.Stock;
                existing.UpdatedAt = DateTime.Now;

                dbContext.ProductSizes.Update(existing);
                await dbContext.SaveChangesAsync();

                return existing; 
            }

            return null;
        }

        public async Task<bool> DeleteByColorIDAsync(int colorID)
        {
            var existing = dbContext.ProductSizes.Where(x => x.ProductColorID == colorID); 
            if (!existing.Any())
            {
                return false;
            }
            dbContext.ProductSizes.RemoveRange(existing);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ProductSizes?> DeleteByColorAndSizeAsync(int colorID, string size)
        {
            if (string.IsNullOrEmpty(size) || colorID == null)
            {
                return null;
            }
            var existing = await dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductColorID == colorID && x.Size == size);
            if (existing == null)
            {
                return null;
            }
            dbContext.ProductSizes.Remove(existing);
            await dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> UpdateRangeAsync(IEnumerable<CartItems> cartItems)
        {
            List<ProductSizes> productSizesUpdate = new List<ProductSizes>();
            foreach (var item in cartItems)
            {
                var productSize = await dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductSizeID == item.ProductSizeID);
                if(productSize == null)
                {
                    return false;
                }
                productSize.Stock -= item.Quantity;
                productSizesUpdate.Add(productSize);
            }
            dbContext.ProductSizes.UpdateRange(productSizesUpdate);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
