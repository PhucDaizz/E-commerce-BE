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
    }
}
