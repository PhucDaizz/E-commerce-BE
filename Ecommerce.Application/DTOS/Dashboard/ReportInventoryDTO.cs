using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Dashboard
{
    public class ReportInventoryDTO
    {
        public int TotalInventory { get; set; }
        public int TotalProductActive { get; set; }
        public int TotalProduct { get; set; }
    }
}
