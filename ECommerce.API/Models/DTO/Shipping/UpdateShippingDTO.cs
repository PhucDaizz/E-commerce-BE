namespace ECommerce.API.Models.DTO.Shipping
{
    public class UpdateShippingDTO
    {
        public string? ShippingServicesID { get; set; }
        public int? ShippingFee { get; set; }
        public string? ShippingStatus { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
    }
}
