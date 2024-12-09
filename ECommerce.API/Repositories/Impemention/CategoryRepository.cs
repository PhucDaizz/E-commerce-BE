using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Impemention
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ECommerceDbContext dbContext;

        public CategoryRepository(ECommerceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Categories> CreateAsync(Categories categories)
        {
            await dbContext.AddAsync(categories);
            await dbContext.SaveChangesAsync();
            return categories;
        }

        public async Task<Categories?> DeleteAsync(int id)
        {
            var existing = await dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryID == id);
            if (existing == null)
            {
                return null;
            }
            else
            {
                dbContext.Categories.Remove(existing);
                await dbContext.SaveChangesAsync();
                return existing;
            }
        }

        public async Task<IEnumerable<Categories>> GetAllAsync()
        {
            var categories = await dbContext.Categories.ToListAsync();
            return categories;
        }

        public async Task<Categories?> GetByIdAsync(int id)
        {
            var existing = await dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryID == id);
            if (existing == null)
            {
                return null;
            }
            return existing;
        }

        public async Task<Categories?> UpdateAsync(Categories categories)
        {
            var existing = await dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryID == categories.CategoryID);
            if (existing != null)
            {
                categories.CreatedAt = existing.CreatedAt;
                categories.UpdatedAt = DateTime.UtcNow;
                dbContext.Entry(existing).CurrentValues.SetValues(categories);
                await dbContext.SaveChangesAsync();
                return existing;
            }
            else
            {
                return null;
            }
        }
    }
}
