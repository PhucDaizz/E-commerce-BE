using Ecommerce.Application.DTOS.Category;
using Ecommerce.Application.DTOS.ProductColor;
using Ecommerce.Application.DTOS.ProductImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Product
{
    public class DetailProductDTO
    {
        public ProductDTO? Product { get; set; }
        public CategoryDTO? Category { get; set; }
        public IEnumerable<ProductColorDTO>? Color { get; set; }
        public IEnumerable<ProductImageDTO> Images { get; set; }
    }
}
