using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories
{
    public class CategoryRepository: ICategoryRepository
    {
        private readonly AppDbContext _dbContext;

        public CategoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Categories> CreateAsync(Categories categories)
        {
            await _dbContext.AddAsync(categories);
            await _dbContext.SaveChangesAsync();
            return categories;
        }

        public async Task<Categories?> DeleteAsync(int id)
        {
            var existing = await _dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryID == id);
            if (existing == null)
            {
                return null;
            }
            else
            {
                _dbContext.Categories.Remove(existing);
                await _dbContext.SaveChangesAsync();
                return existing;
            }
        }

        public async Task<IEnumerable<Categories>> GetAllAsync()
        {
            var categories = await _dbContext.Categories.ToListAsync();
            return categories;
        }

        public async Task<Categories?> GetByIdAsync(int id)
        {
            var existing = await _dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryID == id);
            if (existing == null)
            {
                return null;
            }
            return existing;
        }

        public async Task<Categories?> UpdateAsync(Categories categories)
        {
            var existing = await _dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryID == categories.CategoryID);
            if (existing != null)
            {
                categories.CreatedAt = existing.CreatedAt;
                categories.UpdatedAt = DateTime.UtcNow;
                _dbContext.Entry(existing).CurrentValues.SetValues(categories);
                await _dbContext.SaveChangesAsync();
                return existing;
            }
            else
            {
                return null;
            }
        }
    }
}
