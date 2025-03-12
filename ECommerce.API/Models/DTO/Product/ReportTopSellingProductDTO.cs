using ECommerce.API.Models.DTO.ProductImage;

namespace ECommerce.API.Models.DTO.Product
{
    public class ReportTopSellingProductDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public double Price { get; set; }
        public bool IsPublic { get; set; }
        public int TotalSelling { get; set; }
        public IEnumerable<ProductImageDTO> Images { get; set; }
    }
}
