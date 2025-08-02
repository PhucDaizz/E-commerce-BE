using Ecommerce.Application.DTOS.ProductSize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.ProductColor
{
    public class ProductColorDTO
    {
        public int ProductColorID { get; set; }
        public int ProductID { get; set; }
        public string ColorName { get; set; }
        public string ColorHex { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public IEnumerable<ProductSizeDTO> ProductSizes { get; set; }

    }
}
