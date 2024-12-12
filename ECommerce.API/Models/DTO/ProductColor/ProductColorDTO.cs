namespace ECommerce.API.Models.DTO.ProductColor
{
    public class ProductColorDTO
    {
        public int ProductColorID { get; set; }
        public int ProductID { get; set; }
        public string ColorName { get; set; }
        public string ColorHex { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
