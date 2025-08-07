using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.DTOS.Banner
{
    public class AddBannerImageCommand
    {
        public Banners Banner { get; set; }
        public string FileName { get; set; }
        public bool UseCloudStorage { get; set; }
    }
}
