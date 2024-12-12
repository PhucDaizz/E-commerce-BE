using ECommerce.API.Models.DTO.Category;
using ECommerce.API.Models.DTO.ProductColor;

namespace ECommerce.API.Models.DTO.Product
{
    public class DetailProductDTO
    {
        public ProductDTO? Product { get; set; }
        public CategoryDTO? Category { get; set; }
        public IEnumerable<ProductColorSizeDTO>? Color { get; set; }
    }
}
