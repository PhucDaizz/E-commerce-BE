using ECommerce.API.Models.DTO.ProductSize;

namespace ECommerce.API.Models.DTO.ProductColor
{
    public class ProductColorSizeDTO
    {
        public ProductColorDTO ProductColor { get; set; }
        public IEnumerable<ProductSizeDTO> ProductSize { get; set; }
    }
}
