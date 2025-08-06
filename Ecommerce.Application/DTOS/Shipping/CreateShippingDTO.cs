using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.DTOS.Shipping
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
