namespace Ecommerce.Application.DTOS.Shipping
{
    public class ShippingDTO
    {
        public Guid ShippingID { get; set; }
        public Guid OrderID { get; set; }
        public string? ShippingServicesID { get; set; }
        public int? ShippingFee { get; set; }
        public string ShippingMethod { get; set; } // Standard, Express
        public string ShippingAddress { get; set; }
        public string TrackingNumber { get; set; }
        public string ShippingStatus { get; set; } // 1 = ShippingStatus, 2 = InTransit, 3 = Delivered, 4 = Returned 
        public DateTime EstimatedDeliveryDate { get; set; }
        public DateTime ActualDeliveryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
