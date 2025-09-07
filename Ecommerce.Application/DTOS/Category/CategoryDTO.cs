namespace Ecommerce.Application.DTOS.Category
{
    public class CategoryDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string? ImageURL { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
