using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Product
{
    public class ListProductAdminDTO : ListProductDTO
    {
        public int? TotalQuantity { get; set; }
    }
}
