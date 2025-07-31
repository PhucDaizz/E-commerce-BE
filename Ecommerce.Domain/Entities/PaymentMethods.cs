using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
    public class PaymentMethods
    {
        [Key]
        public int PaymentMethodID { get; set; }
        public string MethodName { get; set; }
        public string? Description { get; set; }

        // Navigation Properties
        public IEnumerable<Orders> Orders { get; set; }
        public IEnumerable<Payments> Payments { get; set; }
    }
}
