namespace Ecommerce.Domain.Entities
{
    public class ProductTags
    {
        public int ProductID { get; set; }
        public int TagID { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Products Products { get; set; }
        public Tags Tags { get; set; }
    }
}
