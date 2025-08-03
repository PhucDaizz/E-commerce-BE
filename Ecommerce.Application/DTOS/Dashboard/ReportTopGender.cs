using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Dashboard
{
    public class ReportTopGender
    {
        public string Gender { get; set; }
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
    }
}
