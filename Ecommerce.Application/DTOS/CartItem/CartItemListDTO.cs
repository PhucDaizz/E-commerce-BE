using Ecommerce.Application.DTOS.Product;
using Ecommerce.Application.DTOS.ProductSize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.CartItem
{
    public class CartItemListDTO
    {
        public int CartItemID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int ProductSizeID { get; set; }
        public ProductImageCartDTO productDTO { get; set; }
        public ProductSizeDTO productSizeDTO { get; set; }
    }
}
