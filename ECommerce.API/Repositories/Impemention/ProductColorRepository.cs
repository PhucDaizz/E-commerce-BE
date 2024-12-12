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
        private readonly ECommerceDbContext dbContext;
        private readonly IMapper mapper;

        public ProductColorRepository(ECommerceDbContext dbContext, IMapper mapper)
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
            var prductColors = await dbContext.ProductColors.ToListAsync();
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

        public async Task<IEnumerable<ProductColors>?> GetAllByProductAsync(int productId)
        {
            var colorsOfProduct = await dbContext.ProductColors.Where(x => x.ProductID == productId).ToListAsync();
            if (!colorsOfProduct.Any())
            {
                return null ;
            }
            return colorsOfProduct;
        }

        public async Task<IEnumerable<ProductColorSizeDTO>> GetProductColorSizeAsync(int productId)
        {
            var productColorSizes = await dbContext.ProductColors
                .Where(pc => pc.ProductID == productId)
                .Select(pc => new ProductColorSizeDTO
                {
                    ProductColor = new ProductColorDTO
                    {
                        ProductColorID = pc.ProductColorID,
                        ProductID = pc.ProductID,
                        ColorName = pc.ColorName,
                        ColorHex = pc.ColorHex,
                        CreatedAt = pc.CreatedAt,
                        UpdatedAt = pc.UpdatedAt
                    },
                    ProductSize = pc.ProductSizes.Select(ps => new ProductSizeDTO
                    {
                        ProductSizeID = ps.ProductSizeID,
                        ProductColorID = ps.ProductColorID,
                        Size = ps.Size,
                        Stock = ps.Stock,
                        CreatedAt = ps.CreatedAt,
                        UpdatedAt = ps.UpdatedAt
                    }).ToList()
                }).ToListAsync();
            if(!productColorSizes.Any())
            {
                return null;
            }
            return productColorSizes;
        }
    }
}
