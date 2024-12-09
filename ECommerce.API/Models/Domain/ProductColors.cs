using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.API.Models.Domain
{
    public class ProductColors
    {
        [Key]
        public int ProductColorID { get; set; }

        [ForeignKey(nameof(Products))]
        public int ProductID { get; set; }

        public string ColorName { get; set; }
        public string ColorHex { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public Products Products { get; set; }
        public ICollection<ProductSizes> ProductSizes { get; set; }
    }
}
