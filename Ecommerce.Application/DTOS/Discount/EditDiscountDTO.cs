using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Discount
{
    public class EditDiscountDTO
    {
        [Required]
        public string Code { get; set; }
        public string Description { get; set; }
        [Required]
        [Range(1, 2, ErrorMessage = "DiscountType must be either 1 (FixedAmount) or 2 (Percentage)")]
        public int DiscountType { get; set; } // Percentage , FixedAmount
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "DiscountValue must be greater than 0")]
        public float DiscountValue { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MaxUsagePerUser must be greater than 0")]
        public int MaxUsagePerUser { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "MinOrderValue must be greater than 0")]
        public double MinOrderValue { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
