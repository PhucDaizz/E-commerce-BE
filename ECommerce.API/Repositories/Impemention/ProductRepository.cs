using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Impemention
{
    public class ProductRepository : IProductRepository
    {
        private readonly ECommerceDbContext dbContext;

        public ProductRepository(ECommerceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Products> CreateAsync(Products products)
        {
            try
            {
                await dbContext.Products.AddAsync(products);
                await dbContext.SaveChangesAsync();
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
            var existing = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == id);
            if (existing == null)
            {
                 return null;
            }
            dbContext.Products.Remove(existing);
            await dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<Products>> GetAllAsync()
        {
            var products = await dbContext.Products.ToListAsync();
            return products;
        }

        public async Task<Products?> GetByIdAsync(int id)
        {
            var existing = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == id);
            if(existing == null)
            {
                return null;
            }
            return existing;
        }

        public async Task<Products?> UpdateAsync(Products products)
        {
            var existing = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == products.ProductID);
            if (existing == null)
            {
                return null;
            }
            products.CreatedAt = existing.CreatedAt;
            dbContext.Products.Entry(existing).CurrentValues.SetValues(products);
            await dbContext.SaveChangesAsync();
            return existing;
        }
    }
}
