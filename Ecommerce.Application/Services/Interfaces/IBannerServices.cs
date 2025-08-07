using Ecommerce.Application.DTOS.Banner;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface IBannerServices
    {
        Task<bool> DeleteBannerAsync(int bannerId);
        Task<Banners> CreateBannerAsync(AddBannerImageCommand command, Stream fileStream);
    }
}
