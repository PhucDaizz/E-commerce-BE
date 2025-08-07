using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Dashboard
{
    public class SalesRevenueDTO
    {
        public double TotalRevenue { get; set; }
        public double RevenueChangePercentage { get; set; }
        public double TotalRevenueThisMonth { get; set; }
    }
}
