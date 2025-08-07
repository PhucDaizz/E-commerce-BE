using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.DTOS.Banner
{
    public class BannerFormModel
    {
        [Required, MaxLength(255)]
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        [MaxLength(500)]
        public string? RedirectUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        [Required]
        public IFormFile ImageFile { get; set; }
        public bool UseCloudStorage { get; set; } = false;
    }
}
