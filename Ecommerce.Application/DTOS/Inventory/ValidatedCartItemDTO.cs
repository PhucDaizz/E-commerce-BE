namespace Ecommerce.Application.DTOS.Inventory
{
    public class ValidatedCartItemDTO
    {
        public int CartItemID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; } 
        public int ProductSizeID { get; set; }
        public string Size { get; set; }
        public double Price { get; set; }
        public int OriginalQuantity { get; set; }
        public int AvailableStock { get; set; }
        public int AdjustedQuantity { get; set; }
    }
}
