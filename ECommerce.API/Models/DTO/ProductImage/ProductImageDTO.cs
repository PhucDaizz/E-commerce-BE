namespace ECommerce.API.Models.DTO.ProductImage
{
    public class ProductImageDTO
    {
        public int ImageID { get; set; }
        public int ProductID { get; set; }
        public string ImageURL { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
