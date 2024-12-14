using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Impemention
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ECommerceDbContext dbContext;

        public ProductImageRepository(ECommerceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<ProductImages> CreateAsync(ProductImages productImages)
        {
            await dbContext.ProductImages.AddAsync(productImages);
            await dbContext.SaveChangesAsync();
            return productImages;
        }

        public async Task<ProductImages?> DeleteAsync(int id)
        {
            var existing = await dbContext.ProductImages.FirstOrDefaultAsync(x => x.ImageID == id);
            if (existing == null)
            {
                return null;
            }
            dbContext.ProductImages.Remove(existing);  
            await dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<ProductImages> GetByIdAsync(int id)
        {
            var existing = await dbContext.ProductImages.FirstOrDefaultAsync(x => x.ImageID == id);
            if (existing == null)
            {
                return null;
            }
            return existing;
        }

        public async Task<ProductImages?> UpdateAsync(ProductImages productImages)
        {
            var existing = await dbContext.ProductImages.FirstOrDefaultAsync(x => x.ImageID == productImages.ImageID);
            if (existing == null)
            {
                return null;
            }
            dbContext.ProductImages.Entry(existing).CurrentValues.SetValues(productImages);
            await dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<ProductImages>> GetAllByProductIDAsync(int productId)
        {
            return await dbContext.ProductImages
                .Where(x => x.ProductID == productId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductImages>> CreateImagesAsync(IEnumerable<ProductImages> imagesList)
        {
            foreach (var image in imagesList)
            {
                dbContext.ProductImages.Add(image);
            }
            await dbContext.SaveChangesAsync();
            return imagesList;
        }
    }
}
