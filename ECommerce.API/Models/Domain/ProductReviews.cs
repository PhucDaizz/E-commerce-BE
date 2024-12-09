using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.API.Models.Domain
{
    public class ProductReviews
    {
        [Key]
        public int ReviewID { get; set; }
        public int ProductID { get; set; }
        public Guid UserID { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("ProductID")]
        public Products Products { get; set; }
    }
}
