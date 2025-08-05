using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Payment
{
    public class PaymentProcessResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
