using ECommerce.API.Models.DTO.Dashboard;
using ECommerce.API.Models.DTO.Product;

namespace ECommerce.API.Services.Interface
{
    public interface IDashboardServices
    {
        Task<SalesRevenueDTO> GetSalesRevenueAsync();
        Task<ReportOrderDTO> GetReportOrderAsync();
        Task<ReportInventoryDTO> GetReportInventoryAsync();
        Task<ReportUserDTO> GetReportUserAsync();
        Task<List<ReportTopSellingProductDTO>> TopSellingProducts(int items);
        Task<List<ReportTopGender>> GetReportTopGenderAsync();


    }
}
