namespace ECommerce.API.Models.DTO.Discount
{
    public class EditDiscountDTO
    {
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
