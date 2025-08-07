using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
    public class Orders
    {
        [Key]
        public Guid OrderID { get; set; }
        public Guid UserID { get; set; }
        public int? DiscountID { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public int PaymentMethodID { get; set; } // 1 = VNPay, 2 = Cash, 3 = PayPal, ...

        public int Status { get; set; } // 0 = Pending , 1 = error, 2 = Completed, 3 = Cancelled, 4 = Confirmed
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public ICollection<OrderDetails> OrderDetails { get; set; }
        public ICollection<Shippings> Shippings { get; set; }

        [ForeignKey("PaymentMethodID")]
        public PaymentMethods PaymentMethods { get; set; }

        [ForeignKey("DiscountID")]
        public Discounts Discounts { get; set; }
        public Payments Payments { get; set; }
    }
}
