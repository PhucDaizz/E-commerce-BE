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
        Task<List<DailyRevenueDTO>> FindDailyRevenue(DateTime startDate, DateTime endDate);
        Task<List<MonthlyRevenueDTO>> FindMonthlyRevenue(int startYear, int startMonth, int endYear, int endMonth);
        Task<List<CustomerLocationDTO>> GetTopCustomerLocationsAsync(int topN = 10);
        Task<List<CustomerLocationDTO>> GetCustomerLocationsByProvinceAsync(string province);

    }
}
