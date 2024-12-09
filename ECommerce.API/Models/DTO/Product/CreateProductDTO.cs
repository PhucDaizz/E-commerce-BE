namespace ECommerce.API.Models.DTO.Product
{
    public class CreateProductDTO
    {
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public double Price { get; set; }
        public string? Description { get; set; }
    }
}
