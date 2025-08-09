using Ecommerce.Application.DTOS.Banner;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface IBannerServices
    {
        Task<bool> DeleteBannerAsync(int bannerId);
        Task<Banners> CreateBannerAsync(AddBannerImageCommand command, Stream? fileStream = null);

        Task<Banners?> UpdateBannerAsync(UpdateBannerCommand command, Stream? fileStream = null);
    }
}
