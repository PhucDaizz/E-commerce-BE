using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IBannerRepository
    {
        Task<Banners> GetByIdAsync(int id);
        Task<IEnumerable<Banners>> GetAllAsync(bool onlyActive = true);
        Task<Banners> AddAsync(Banners banner);
        void Update(Banners banner);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangeStatusAsync(int id);
    }
}
