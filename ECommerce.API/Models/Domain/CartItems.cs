using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.API.Models.Domain
{
    public class CartItems
    {
        [Key]
        public int CartItemID { get; set; }
        public Guid UserID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int ProductSizeID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("ProductID")]
        public Products Products { get; set; }

        [ForeignKey("ProductSizeID")]
        public ProductSizes ProductSizes { get; set; }
    }
}
