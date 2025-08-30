using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.DTOS.Banner
{
    public class UpdateBannerCommand
    {
        public Banners Banner { get; set; }
        public string FileName { get; set; }
        public bool UseCloudStorage { get; set; }
        public bool HasNewImage { get; set; }
    }
}
