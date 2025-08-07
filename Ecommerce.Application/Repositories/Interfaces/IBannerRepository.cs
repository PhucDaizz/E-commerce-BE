using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IBannerRepository
    {
        Task<Banners> GetByIdAsync(int id);
        Task<IEnumerable<Banners>> GetAllAsync(bool onlyActive = true);
        Task<Banners> AddAsync(Banners banner);
        Task<bool> UpdateAsync(Banners banner);
        Task<bool> DeleteAsync(int id);
    }
}
