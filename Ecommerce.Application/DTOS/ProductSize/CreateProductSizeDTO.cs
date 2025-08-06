using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.ProductSize
{
    public class CreateProductSizeDTO
    {
        public int ProductColorID { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }
    }
}
