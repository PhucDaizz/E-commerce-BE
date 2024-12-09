using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.API.Models.Domain
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailID { get; set; }
        public Guid OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }

        // Navigation Properties
        [ForeignKey("ProductID")]
        public Products Products { get; set; }
        public Orders Orders { get; set; } 

    }
}
