using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;

namespace ECommerce.API.Repositories.Impemention
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext dbContext;
        private readonly IHostEnvironment environment;

        public ProductImageRepository(AppDbContext dbContext, IHostEnvironment environment)
        {
            this.dbContext = dbContext;
            this.environment = environment;
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

        /*public async Task<bool> retainProductFeaturedImage(IEnumerable<ProductImages> productImages)
        {
            *//*var images = await dbContext.ProductImages.Where(x => x.ProductID == productId).OrderBy(x => x.CreatedAt).Skip(1).ToListAsync();
            
            foreach (var image in images)
            {
                if (!string.IsNullOrEmpty(image.ImageURL))
                {
                    var contentPath = environment.ContentRootPath;
                    var path = Path.Combine(contentPath, "Uploads", image.ImageURL);
                
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }*//*

            dbContext.ProductImages.RemoveRange(productImages);

            await dbContext.SaveChangesAsync();

            return true;
        }*/

        public async Task<bool> DeleteProductImagesAsync(int productId)
        {
            var images = await dbContext.ProductImages.Where(x => x.ProductID == productId).OrderBy(x => x.CreatedAt).ToListAsync();

            foreach (var image in images)
            {
                if (!string.IsNullOrEmpty(image.ImageURL))
                {
                    var contentPath = environment.ContentRootPath;
                    var path = Path.Combine(contentPath, "Uploads", image.ImageURL);

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }

            dbContext.ProductImages.RemoveRange(images);

            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
