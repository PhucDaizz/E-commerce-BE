using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models.DTO.Shipping
{
    public class CreateShippingDTO
    {
        [Required]
        public Guid OrderID { get; set; }
        [Required]
        public string ShippingMethod { get; set; } // Standard, Express
        [Required]
        public string ShippingAddress { get; set; }
        
    }
}
