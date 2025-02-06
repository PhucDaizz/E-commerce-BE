using ECommerce.API.Models.DTO.ProductImage;

namespace ECommerce.API.Models.DTO.Product
{
    public class ProductImageCartDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public IEnumerable<ProductImageDTO> Images { get; set; }
    }
}
