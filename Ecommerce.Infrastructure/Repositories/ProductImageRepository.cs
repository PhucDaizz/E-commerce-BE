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
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext dbContext;

        public ProductImageRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<ProductImages> CreateAsync(ProductImages productImages)
        {
            await dbContext.ProductImages.AddAsync(productImages);
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

        public async Task<int> CountByProductIdAsync(int id)
        {
            return await dbContext.ProductImages.CountAsync(x => x.ProductID == id);
        }
    }
}
