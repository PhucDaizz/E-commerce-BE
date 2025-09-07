using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.DTOS.Category
{
    public class UpdateCategoryFormModel
    {
        [Required]
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public IFormFile? ImageFile { get; set; }
        public bool UseCloudStorage { get; set; } = false;
    }
}
