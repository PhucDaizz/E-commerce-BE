using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Shipping
{
    public class UpdateShippingDTO
    {
        public string? ShippingServicesID { get; set; }
        public int? ShippingFee { get; set; }
        public string? ShippingStatus { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
    }
}
