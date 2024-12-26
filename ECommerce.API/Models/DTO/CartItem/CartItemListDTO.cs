using ECommerce.API.Models.DTO.Product;

namespace ECommerce.API.Models.DTO.CartItem
{
    public class CartItemListDTO
    {
        public int CartItemID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int ProductSizeID { get; set; }

        public ProductDTO productDTO { get; set; }
    }
}
