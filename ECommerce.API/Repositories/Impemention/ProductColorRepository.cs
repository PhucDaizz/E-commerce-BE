using AutoMapper;
using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.ProductColor;
using ECommerce.API.Models.DTO.ProductSize;
using ECommerce.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Impemention
{
    public class ProductColorRepository : IProductColorRepository
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;

        public ProductColorRepository(AppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<ProductColors> CreateAsync(ProductColors productColors)
        {
            await dbContext.ProductColors.AddAsync(productColors);
            await dbContext.SaveChangesAsync();
            return productColors;
        }

        public async Task<ProductColors?> DeleteAsync(int id)
        {
            var existing = await dbContext.ProductColors.FirstOrDefaultAsync(x => x.ProductColorID == id);
            if (existing == null)
            {
                return null;
            }
            dbContext.ProductColors.Remove(existing);
            await dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<ProductColors>> GetAllAsync()
        {
            var prductColors = await dbContext.ProductColors.Include(x => x.ProductSizes).ToListAsync();
            return prductColors;
        }

        public async Task<ProductColors?> GetByIdAsync(int id)
        {
            var productColor = await dbContext.ProductColors.Include(x => x.ProductSizes).FirstOrDefaultAsync(x => x.ProductColorID == id);
            if(productColor == null)
            {
                return null; 
            }
            return productColor;
        }

        public async Task<ProductColors?> UpdateAsync(ProductColors productColors)
        {
            var existing = await dbContext.ProductColors.FirstOrDefaultAsync(x => x.ProductColorID == productColors.ProductColorID);
            if (existing == null)
            {
                return null;
            }
            productColors.CreatedAt = existing.CreatedAt;
            dbContext.ProductColors.Entry(existing).CurrentValues.SetValues(productColors);
            await dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<ProductColors>> GetProductColorSizeAsync(int productId)
        {
            var productColorSizes = await dbContext.ProductColors.Include(x => x.ProductSizes)
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
            await dbContext.ProductColors.AddRangeAsync(productColors);
            await dbContext.SaveChangesAsync();
            return productColors;
        }

        public async Task<bool> DeleteProductColorSizeAsync(int productId)
        {
             var colors = await dbContext.ProductColors
                .Include(x => x.ProductSizes)
                .Where(x => x.ProductID == productId)
                .ToListAsync();

            if (colors == null)
            {
                return false;
            }

            foreach (var color in colors)
            {
                dbContext.ProductSizes.RemoveRange(color.ProductSizes);
                dbContext.ProductColors.Remove(color);
            }

            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
