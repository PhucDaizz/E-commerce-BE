namespace ECommerce.API.Models.DTO.ProductReview
{
    public class ProductReviewDTO
    {
        public int ReviewID { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Username { get; set; }
    }
}
