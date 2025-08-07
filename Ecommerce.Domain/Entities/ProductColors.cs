using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
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
