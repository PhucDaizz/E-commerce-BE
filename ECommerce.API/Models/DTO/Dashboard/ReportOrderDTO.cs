namespace ECommerce.API.Models.DTO.Dashboard
{
    public class ReportOrderDTO
    {
        public int TotalOrder { get; set; }
        public float OrderChangePercentage { get; set; }
        public int TotalOrderThisMonth { get; set; }
    }
}
