using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
    public class Discounts
    {
        [Key]
        public int DiscountID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int DiscountType { get; set; } //  1 = FixedAmount, 2 = Percentage 
        public float DiscountValue { get; set; }
        public int Quantity { get; set; }
        public int MaxUsagePerUser { get; set; }
        public double MinOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation Properties
        public ICollection<Orders> Orders { get; set; }
    }
}
