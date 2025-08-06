using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.DTOS.Discount
{
    public class CreateDiscountDTO
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
        public int Quantity { get; set; }
        [Required]
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
