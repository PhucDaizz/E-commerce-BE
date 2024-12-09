using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models.Domain
{
    public class PaymentMethods
    {
        [Key]
        public int PaymentMethodID { get; set; }
        public string MethodName { get; set; }
        public string? Description { get; set; }

        // Navigation Properties
        public IEnumerable<Orders> Orders { get; set; }
    }
}
