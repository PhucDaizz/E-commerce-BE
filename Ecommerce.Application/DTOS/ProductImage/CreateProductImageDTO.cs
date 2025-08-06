using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.DTOS.ProductImage
{
    public class CreateProductImageDTO
    {
        [Required]
        public int ProductID { get; set; }

        [Required]
        public IFormFile ImageURL { get; set; }

        [Required]
        public bool IsPrimary { get; set; } = false;

        public bool OnCloud { get; set; } = false;
    }
}
