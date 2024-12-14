using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models.DTO.ProductImage
{
    public class CreateProductImageDTO
    {
        [Required]
        public int ProductID { get; set; }

        [Required]
        public IFormFile ImageURL { get; set; }

        [Required]
        public bool IsPrimary { get; set; } = false;
    }
}
