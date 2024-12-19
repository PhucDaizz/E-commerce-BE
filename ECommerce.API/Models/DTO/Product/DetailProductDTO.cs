using ECommerce.API.Models.DTO.Category;
using ECommerce.API.Models.DTO.ProductColor;
using ECommerce.API.Models.DTO.ProductImage;

namespace ECommerce.API.Models.DTO.Product
{
    public class DetailProductDTO
    {
        public ProductDTO? Product { get; set; }
        public CategoryDTO? Category { get; set; }
        public IEnumerable<ProductColorDTO>? Color { get; set; }
        public IEnumerable<ProductImageDTO> Images { get; set; }
    }
}
