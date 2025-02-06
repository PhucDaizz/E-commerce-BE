using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.API.Models.Domain
{
    public class Products
    {
        [Key]
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public double Price { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsPublic { get; set; } = false;

        // Navigation Properties
        [ForeignKey("CategoryID")]
        public Categories Categories { get; set; }
        public ICollection<ProductColors> ProductColors { get; set; }
        public ICollection<ProductImages> ProductImages { get; set; }
        public ICollection<ProductReviews> ProductReviews { get; set; }
        public ICollection<CartItems> CartItems { get; set; }
    }
}
