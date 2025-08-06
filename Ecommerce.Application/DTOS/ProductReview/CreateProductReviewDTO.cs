using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.DTOS.ProductReview
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
