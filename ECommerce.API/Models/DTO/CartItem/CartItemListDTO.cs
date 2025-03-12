using ECommerce.API.Models.DTO.Product;
using ECommerce.API.Models.DTO.ProductSize;

namespace ECommerce.API.Models.DTO.CartItem
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
