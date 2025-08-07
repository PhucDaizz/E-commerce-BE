using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.ProductSize
{
    public class GetProductSizeDTO
    {
        public int ProductSizeID { get; set; }
        public string ColorName { get; set; }
        public string Size { get; set; }
    }
}
