using Ecommerce.Application.DTOS.ProductImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Dashboard
{
    public class ReportTopSellingProductDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public double Price { get; set; }
        public bool IsPublic { get; set; }
        public int TotalSelling { get; set; }
        public IEnumerable<ProductImageDTO> Images { get; set; }
    }
}
