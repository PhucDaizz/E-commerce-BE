namespace ECommerce.API.Models.DTO.CartItem
{
    public class EditCartItemDTO
    {
        public int CartItemID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int ProductSizeID { get; set; }
    }
}
