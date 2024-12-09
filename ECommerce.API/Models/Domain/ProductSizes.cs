using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.API.Models.Domain
{
    public class ProductSizes
    {
        [Key]
        public int ProductSizeID{ get; set; }
        public int ProductColorID { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("ProductColorID")]
        public ProductColors ProductColors { get; set; }
    }
}
