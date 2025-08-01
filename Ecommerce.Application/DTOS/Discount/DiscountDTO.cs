using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Discount
{
    public class DiscountDTO
    {
        public int DiscountID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int DiscountType { get; set; } // Percentage , FixedAmount
        public float DiscountValue { get; set; }
        public int Quantity { get; set; }
        public int MaxUsagePerUser { get; set; }
        public double MinOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
