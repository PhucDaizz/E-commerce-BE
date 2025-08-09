using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories
{
    public class BannerRepository : IBannerRepository
    {
        private readonly AppDbContext _dbContext;

        public BannerRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Banners> AddAsync(Banners banner)
        {
            var newBanner  = await _dbContext.Banners.AddAsync(banner);
            return newBanner.Entity;
        }

        public async Task<bool> ChangeStatusAsync(int id)
        {
            return await _dbContext.Banners.Where(b => b.Id == id)
                .ExecuteUpdateAsync(b => b
                    .SetProperty(b => b.IsActive, b => !b.IsActive)
                    .SetProperty(b => b.UpdatedAt, b => DateTime.UtcNow)
                ) > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var banner = await _dbContext.Banners.FirstOrDefaultAsync(x => x.Id == id);
            if (banner != null)
            {
                _dbContext.Banners.Remove(banner);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Banners>> GetAllAsync(bool onlyActive = true)
        {
            var query = _dbContext.Banners.AsQueryable();
            if (onlyActive)
            {
                query = query.Where(b => b.IsActive &&
                    (!b.StartDate.HasValue || b.StartDate <= DateTime.UtcNow) &&
                    (!b.EndDate.HasValue || b.EndDate >= DateTime.UtcNow));
            }
            return await query.OrderBy(b => b.DisplayOrder).ToListAsync();
        }

        public async Task<Banners> GetByIdAsync(int id)
        {
            return await _dbContext.Banners
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public void Update(Banners banner)
        {
            _dbContext.Banners.Update(banner);
        }
    }
}
