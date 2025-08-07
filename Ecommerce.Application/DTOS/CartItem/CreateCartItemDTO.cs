namespace Ecommerce.Application.DTOS.CartItem
{
    public class CreateCartItemDTO
    {
        /*public Guid UserID { get; set; }*/
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int ProductSizeID { get; set; }
    }
}
