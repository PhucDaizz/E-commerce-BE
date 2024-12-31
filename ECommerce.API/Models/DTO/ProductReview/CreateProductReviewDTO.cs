using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models.DTO.ProductReview
{
    public class CreateProductReviewDTO
    {
        [Required]
        public int ProductID { get; set; }

        [Required]
        [Range(1, 5)]
        public double Rating { get; set; }

        [Required]
        [MinLength(10)]
        public string Comment { get; set; }
    }
}
