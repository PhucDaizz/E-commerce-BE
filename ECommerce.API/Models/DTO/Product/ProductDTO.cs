using ECommerce.API.Models.DTO.Category;
using ECommerce.API.Models.DTO.ProductColor;

namespace ECommerce.API.Models.DTO.Product
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public double Price { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
