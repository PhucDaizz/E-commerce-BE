using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
    public class Shippings
    {
        [Key]
        public Guid ShippingID { get; set; }
        public Guid OrderID { get; set; }
        public string? ShippingServicesID { get; set; }
        public int? ShippingFee { get; set; }
        public string? ShippingMethod { get; set; } // Standard, Express
        public string? ShippingAddress { get; set; }
        public string? TrackingNumber { get; set; }
        public string? ShippingStatus { get; set; } // 1 = InTransit, 2 = Delivered, 3 = Returned 
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("OrderID")]
        public Orders Orders { get; set; }
    }
}
