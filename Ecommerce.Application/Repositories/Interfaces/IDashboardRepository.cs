using Ecommerce.Application.DTOS.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<ReportInventoryDTO> GetReportInventoryAsync();
        Task<ReportOrderDTO> GetReportOrderAsync();
        Task<List<ReportTopGender>> GetReportTopGenderAsync();
        Task<ReportUserDTO> GetReportUserAsync();
        Task<SalesRevenueDTO> GetSalesRevenueAsync();
        Task<List<ReportTopSellingProductDTO>> TopSellingProductsAsync(int items);
    }
}
