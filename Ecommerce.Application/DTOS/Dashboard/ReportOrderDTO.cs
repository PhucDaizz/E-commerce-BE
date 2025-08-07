using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Dashboard
{
    public class ReportOrderDTO
    {
        public int TotalOrder { get; set; }
        public float OrderChangePercentage { get; set; }
        public int TotalOrderThisMonth { get; set; }
    }
}
