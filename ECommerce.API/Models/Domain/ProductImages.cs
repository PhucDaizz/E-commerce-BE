using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.API.Models.Domain
{
    public class ProductImages
    {
        [Key]
        public int ImageID { get; set; }
        public int ProductID { get; set; }
        public string ImageURL { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("ProductID")]
        public Products Products { get; set; }

    }
}
