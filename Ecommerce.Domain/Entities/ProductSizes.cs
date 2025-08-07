using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
    public class ProductSizes
    {
        [Key]
        public int ProductSizeID { get; set; }
        public int ProductColorID { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("ProductColorID")]
        public ProductColors ProductColors { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
        public ICollection<CartItems> CartItems { get; set; }
    }
}
