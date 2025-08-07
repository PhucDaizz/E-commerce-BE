using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.ProductSize
{
    public class CreateProductSizesDTO
    {
        public Dictionary<int, Dictionary<string, int>> ProductSizes { get; set; }
    }
}
