using ECommerce.API.Models.DTO.OrderDetail;
using ECommerce.API.Models.DTO.Payment;
using ECommerce.API.Models.DTO.Shipping;

namespace ECommerce.API.Models.DTO.Order
{
    public class GetDetailOrderDTO
    {
        public Guid OrderID { get; set; }
        public Guid UserID { get; set; }
        public int? DiscountID { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public int PaymentMethodID { get; set; } // 1 = Cash, 2 = CreditCard, 3 = PayPal, ...
        public int Status { get; set; } // 1 = Pending , 2 = Confirmed, 3 = Cancelled, 4 = Completed 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PaymentDTO PaymentDTO { get; set; }
        public List<ShippingDTO> ShippingDTO { get; set; }
        public List<GetOrderDetailDTO> GetOrderDetailDTO { get; set; }

    }
}
