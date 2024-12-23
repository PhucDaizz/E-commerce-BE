namespace ECommerce.API.Models.DTO.Order
{
    public class CreateOrderDTO
    {
        public Guid UserID { get; set; }
        public int DiscountID { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public int PaymentMethodID { get; set; } // 1 = Cash, 2 = CreditCard, 3 = PayPal, ...
        public int Status { get; set; } // 1 = Pending , 2 = Confirmed, 3 = Cancelled, 4 = Completed 
    }
}
