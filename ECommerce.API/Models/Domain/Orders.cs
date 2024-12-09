using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.API.Models.Domain
{
    public class Orders
    {
        [Key]
        public Guid OrderID { get; set; }
        public Guid UserID { get; set; }
        public int DiscountID { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public int PaymentMethodID { get; set; } // 1 = Cash, 2 = CreditCard, 3 = PayPal, ...

        public int Status { get; set; } // 1 = Pending , 2 = Confirmed, 3 = Cancelled, 4 = Completed 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public ICollection<OrderDetails> OrderDetails { get; set; }
        public ICollection<Shippings>Shippings { get; set; }

        [ForeignKey("PaymentMethodID")]
        public PaymentMethods PaymentMethods { get; set; }

        [ForeignKey("DiscountID")]
        public Discounts Discounts { get; set; }

    }
}
