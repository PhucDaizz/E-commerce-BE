using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.ProductColor
{
    public class CreateProductColorDTO
    {
        public int ProductID { get; set; }
        public string ColorName { get; set; }
        public string ColorHex { get; set; }
    }
}
