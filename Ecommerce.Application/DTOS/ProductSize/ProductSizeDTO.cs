using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.ProductSize
{
    public class ProductSizeDTO
    {
        public int ProductSizeID { get; set; }
        public int ProductColorID { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
